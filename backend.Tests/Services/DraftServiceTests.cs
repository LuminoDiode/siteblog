using backend.Models.Runtime;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore.Metadata;

namespace backend.Tests.Services
{
	public class DraftServiceTests
	{
		ITestOutputHelper _output;
		ILogger<DraftService<object, DbContext, int>> _logger;

		public Mock<SettingsProviderService> defaultSettingsProviderMock;
		public DraftService<object, DbContext, int> defaultDraftService;
		public DraftServiceSettings defaultSettings;

		public DraftServiceTests(ITestOutputHelper output)
		{
			_logger = output.BuildLoggerFor<DraftService<object, DbContext, int>>();
			_output = output;

			defaultSettings = new DraftServiceSettings
			{
				maxEntitiesStoredInDb = byte.MaxValue,
				maxEntitiesStoredInRam = byte.MaxValue,
				maxTimeEntityStoredInDbMinutes = byte.MaxValue,
				maxTimeEntityStoredInRamMinutes = byte.MaxValue,
				updateStoredEntitiesIntervalMinutes = byte.MaxValue
			};

			defaultSettingsProviderMock = new Mock<SettingsProviderService>(null);
			defaultSettingsProviderMock.SetupGet(x => x.DraftServiceSettings).Returns(defaultSettings);

			defaultDraftService = new(defaultSettingsProviderMock.Object, null, null, null, null, null, _logger);
		}

		[Fact]
		public void CanCreateInstance()
		{
			Assert.NotNull(defaultDraftService);
		}

		[Fact]
		public async Task CanAdd()
		{
			var service = defaultDraftService;
			await service.StartAsync(new CancellationToken());

			int a = 1;
			int b = 2;
			int c = 3;

			service.PushDraft(a);
			service.PushDraft(b);

			var aRetrieved = await service.GetDraftAsync(x => x.Equals(a));
			var bRetrieved = await service.GetDraftAsync(x => x.Equals(b));
			var nullRetrieved = await service.GetDraftAsync(x => x.Equals(c));

			Assert.Equal(a, aRetrieved);
			Assert.Equal(b, bRetrieved);
			Assert.Null(nullRetrieved);
		}

		[Fact]
		public async Task CanRemoveOutdatedFromRam()
		{
			var mySettings = new DraftServiceSettings
			{
				maxEntitiesStoredInDb = int.MaxValue,
				maxEntitiesStoredInRam = int.MaxValue,
				maxTimeEntityStoredInDbMinutes = int.MaxValue,
				maxTimeEntityStoredInRamMinutes = 1d / 60d, // 1 s
				updateStoredEntitiesIntervalMinutes = 1d / 60 / 100 // 10 ms
			};

			defaultSettingsProviderMock.SetupGet(x => x.DraftServiceSettings).Returns(mySettings);
			var service = new DraftService<object, DbContext, int>(defaultSettingsProviderMock.Object, null, null, null, null, null, _logger);
			await service.StartAsync(new CancellationToken(false));

			int a = 1;
			int b = 2;			

			service.PushDraft(a);
			await Task.Delay(1100); // + 100 ms for ensure, a should be deleted after delay
			service.PushDraft(b);

			var aRetrieved = await service.GetDraftAsync(x => x.Equals(a));
			var bRetrieved = await service.GetDraftAsync(x => x.Equals(b));

			Assert.Null(aRetrieved);
			Assert.Equal(b, bRetrieved);

			await Task.Delay(1100); // b should be deleted after delay

			bRetrieved = await service.GetDraftAsync(x => x.Equals(b));

			Assert.Null(bRetrieved);
		}

		// there is no field for testing this, so see the logs
		[Fact]
		public async Task WillDecreaseCapacity()
		{
			var mySettings = new DraftServiceSettings
			{
				maxEntitiesStoredInDb = int.MaxValue,
				maxEntitiesStoredInRam = int.MaxValue,
				maxTimeEntityStoredInDbMinutes = int.MaxValue,
				maxTimeEntityStoredInRamMinutes = 1d / 600d, // 0.1 s
				updateStoredEntitiesIntervalMinutes = 1d / 60 / 100 // 10 ms
			};

			defaultSettingsProviderMock.SetupGet(x => x.DraftServiceSettings).Returns(mySettings);
			var service = new DraftService<object, DbContext, int>(defaultSettingsProviderMock.Object, null, null, null, null, null, _logger);

			for(int i = 0; i < short.MaxValue + 1; i++)
			{
				service.PushDraft(i);
			}

			await service.StartAsync(new CancellationToken(false));
			await Task.Delay(110);

			Assert.Null(await service.GetDraftAsync(x => x.Equals(short.MaxValue)));
		}
	}
}
