using Ovenbird.Core.ContributorAggregate;
using Vogen;

namespace Ovenbird.Infrastructure.Data.Config;

[EfCoreConverter<ContributorId>]
[EfCoreConverter<ContributorName>]
internal partial class VogenEfCoreConverters;
