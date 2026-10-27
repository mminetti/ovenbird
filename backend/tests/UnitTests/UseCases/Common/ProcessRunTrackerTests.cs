using Ardalis.Result;
using Core.Common.Constants;
using Core.Processing;
using UseCases.Common;

namespace UnitTests.UseCases.Common;

public class ProcessRunTrackerTests
{
    private readonly IRepository<ProcessRun> _processRunRepository = Substitute.For<IRepository<ProcessRun>>();
    private readonly IRepository<ProcessRunLogItem> _logItemRepository = Substitute.For<IRepository<ProcessRunLogItem>>();
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

        _tracker = new ProcessRunTracker(_processRunRepository, _logItemRepository, TimeProvider.System);
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
    public async Task CompleteAsync_PersistsLogItemsParsedFromEachValidationError_AndCompletesTheRun()
    {
        _processRunRepository.GetByIdAsync(1L, Arg.Any<CancellationToken>())
            .Returns(new ProcessRun { Id = 1, StatusId = Constants.ProcessRunStatuses.Running });

        var errors = new[]
        {
            new ValidationError(
                EntityReference.Format("MarketDocumentItem", 500), "Unmapped customer class 'Residential'.", "CustomerClass", ValidationSeverity.Error),
            new ValidationError(string.Empty, "Skipped a duplicate document.", string.Empty, ValidationSeverity.Warning),
        };

        await _tracker.CompleteAsync(1, succeeded: false, errors, CancellationToken.None);

        await _logItemRepository.Received(1).AddRangeAsync(
            Arg.Is<IEnumerable<ProcessRunLogItem>>(items => items.Count() == 2
                && items.Any(i => i.EntityType == "MarketDocumentItem" && i.EntityId == "500"
                    && i.Field == "CustomerClass" && i.LevelId == Constants.ProcessRunLogLevels.Error)
                && items.Any(i => i.EntityType == null && i.EntityId == null
                    && i.Field == string.Empty && i.LevelId == Constants.ProcessRunLogLevels.Warning)),
            Arg.Any<CancellationToken>());

        await _processRunRepository.Received(1).UpdateAsync(
            Arg.Is<ProcessRun>(r => r.StatusId == Constants.ProcessRunStatuses.Failed && r.CompletedAtUtc != null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompleteAsync_WithNoErrors_StillCompletesTheRunAsSucceeded_WithoutPersistingLogItems()
    {
        _processRunRepository.GetByIdAsync(1L, Arg.Any<CancellationToken>())
            .Returns(new ProcessRun { Id = 1, StatusId = Constants.ProcessRunStatuses.Running });

        await _tracker.CompleteAsync(1, succeeded: true, [], CancellationToken.None);

        await _logItemRepository.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<ProcessRunLogItem>>(), Arg.Any<CancellationToken>());

        await _processRunRepository.Received(1).UpdateAsync(
            Arg.Is<ProcessRun>(r => r.StatusId == Constants.ProcessRunStatuses.Succeeded),
            Arg.Any<CancellationToken>());
    }
}
