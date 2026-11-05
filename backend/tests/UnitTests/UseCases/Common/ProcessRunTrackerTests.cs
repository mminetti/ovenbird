using Ardalis.Result;
using Core.Common.Constants;
using Core.Processing;
using UseCases.Common;
using Case = Core.Common.Case;

namespace UnitTests.UseCases.Common;

public class ProcessRunTrackerTests
{
    private readonly IRepository<ProcessRun> _processRunRepository = Substitute.For<IRepository<ProcessRun>>();
    private readonly IRepository<Case> _caseRepository = Substitute.For<IRepository<Case>>();
    private readonly ProcessRunTracker _tracker;

    public ProcessRunTrackerTests()
    {
        _processRunRepository
            .AddAsync(Arg.Any<ProcessRun>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var run = callInfo.Arg<ProcessRun>();
                run.Id = 1;
                return run;
            });

        _tracker = new ProcessRunTracker(_processRunRepository, _caseRepository, TimeProvider.System);
    }

    [Fact]
    public async Task StartAsync_PersistsARunningRunForTheGivenProcessType()
    {
        var run = await _tracker.StartAsync(Constants.ProcessTypes.MarketDocumentImport, CancellationToken.None);

        run.Id.ShouldBe(1);
        run.ProcessTypeId.ShouldBe(Constants.ProcessTypes.MarketDocumentImport);
        run.StatusId.ShouldBe(Constants.ProcessRunStatuses.Running);
    }

    [Fact]
    public async Task CompleteAsync_RaisesACaseWithOneItemPerErrorSeverityValidationError_AndCompletesTheRun()
    {
        _processRunRepository.GetByIdAsync(1L, Arg.Any<CancellationToken>())
            .Returns(new ProcessRun { Id = 1, StatusId = Constants.ProcessRunStatuses.Running });

        var errors = new[]
        {
            new ValidationError("CustomerClass", "Unmapped customer class 'Residential'.", string.Empty, ValidationSeverity.Error),
            new ValidationError(string.Empty, "Skipped a duplicate document.", string.Empty, ValidationSeverity.Warning),
        };

        await _tracker.CompleteAsync(1, errors, CancellationToken.None);

        await _caseRepository.Received(1).AddAsync(
            Arg.Is<Case>(c => c.EntityTypeId == Constants.CaseEntityTypes.ProcessRun
                && c.EntityId == 1
                && c.StatusId == Constants.CaseStatuses.Open
                && c.Items.Count == 1
                && c.Items.Any(i => i.Field == "CustomerClass" && i.Message == "Unmapped customer class 'Residential'."
                    && i.SeverityId == Constants.ErrorSeverities.Error)),
            Arg.Any<CancellationToken>());

        await _processRunRepository.Received(1).UpdateAsync(
            Arg.Is<ProcessRun>(r => r.StatusId == Constants.ProcessRunStatuses.Failed && r.CompletedAtUtc != null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompleteAsync_WithNoErrors_StillCompletesTheRunAsSucceeded_WithoutRaisingACase()
    {
        _processRunRepository.GetByIdAsync(1L, Arg.Any<CancellationToken>())
            .Returns(new ProcessRun { Id = 1, StatusId = Constants.ProcessRunStatuses.Running });

        await _tracker.CompleteAsync(1, [], CancellationToken.None);

        await _caseRepository.DidNotReceive().AddAsync(Arg.Any<Case>(), Arg.Any<CancellationToken>());

        await _processRunRepository.Received(1).UpdateAsync(
            Arg.Is<ProcessRun>(r => r.StatusId == Constants.ProcessRunStatuses.Succeeded),
            Arg.Any<CancellationToken>());
    }
}
