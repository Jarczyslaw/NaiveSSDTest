using JToolbox.AppConfig;
using System;

namespace NaiveSSDTest.Services
{
    public class AppConfig : AppConfigService
    {
        public int TasksCount => Convert.ToInt32(GetValue());

    }
}