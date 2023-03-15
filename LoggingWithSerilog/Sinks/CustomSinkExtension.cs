using Serilog;
using Serilog.Configuration;

namespace LoggingWithSerilog.Sinks;

public static class CustomSinkExtension
{
    public static LoggerConfiguration CustomSink(this LoggerSinkConfiguration loggerSinkConfiguration)
    {
        return loggerSinkConfiguration.Sink(new CustomSink());
    }
}