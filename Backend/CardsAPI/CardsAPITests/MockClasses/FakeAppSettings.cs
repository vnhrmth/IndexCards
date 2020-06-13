using System;
using CardsAPI.Helper;
using Microsoft.Extensions.Options;

namespace CardsAPITests.MockClasses
{
    public class FakeAppSettings : IOptions<AppSettings>
    {
        AppSettings IOptions<AppSettings>.Value => new AppSettings() { Secret = "DDDDDDDD-dddd-4152-855C-9c0f6141bfd9" };
    }
}
