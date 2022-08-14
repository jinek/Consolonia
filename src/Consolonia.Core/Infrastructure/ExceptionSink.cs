using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Logging;

namespace Consolonia.Core.Infrastructure;

public class ExceptionSink : ILogSink
{
    public bool IsEnabled(LogEventLevel level, string area)
    {
        return level is LogEventLevel.Error or LogEventLevel.Fatal;
    }

    public void Log(LogEventLevel level, string area, object source, string messageTemplate)
    {
        Log(level, area, source, messageTemplate, Array.Empty<object>());
    }

    public void Log<T0>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0)
    {
        Log(level, area, source, messageTemplate, new object[] { propertyValue0 });
    }

    public void Log<T0, T1>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1)
    {
        Log(level, area, source, messageTemplate, new object[] { propertyValue0, propertyValue1 });
    }

    public void Log<T0, T1, T2>(LogEventLevel level, string area, object source, string messageTemplate,
        T0 propertyValue0,
        T1 propertyValue1, T2 propertyValue2)
    {
        Log(level, area, source, messageTemplate, new object[] { propertyValue0, propertyValue1, propertyValue2 });
    }

    public void Log(LogEventLevel level, string area, object source, string messageTemplate,
        params object[] propertyValues)
    {
        var consoloniaException =
            new ConsoloniaException($"{area}: " +
                                    string.Format(CultureInfo.CurrentCulture, messageTemplate, propertyValues))
            {
                Source = source?.ToString()
            };

        for (int i = 0; i < propertyValues.Length; i++)
        {
            object propertyValue = propertyValues[i];
            consoloniaException.Data.Add(i, propertyValue);
        }

        throw consoloniaException;
    }
}

public static class ExceptionSinkExtensions
{
    public static T LogToException<T>(this T builder) where T : AppBuilderBase<T>, new()
    {
        Logger.Sink = new ExceptionSink();
        return builder;
    }
}