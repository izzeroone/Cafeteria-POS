using System;
using System.Diagnostics;
using System.Text;

namespace Cafocha.Context.Logging
{
    public class Logger : ILogger
    {
        public void Information(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Information(string fmt, params object[] vars)
        {
            Trace.TraceInformation(fmt, vars);
        }

        public void Information(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceInformation(FormatExceptionMessage(exception, fmt, vars));
        }

        public void Warning(string message)
        {
            Trace.TraceWarning(message);
        }

        public void Warning(string fmt, params object[] vars)
        {
            Trace.TraceWarning(fmt, vars);
        }

        public void Warning(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceWarning(FormatExceptionMessage(exception, fmt, vars));
        }

        public void Error(string message)
        {
            Trace.TraceError(message);
        }

        public void Error(string fmt, params object[] vars)
        {
            Trace.TraceError(fmt, vars);
        }

        public void Error(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceError(FormatExceptionMessage(exception, fmt, vars));
        }

        public void TraceApi(string componentName, string method, TimeSpan timeSpan)
        {
            TraceApi(componentName, method, timeSpan, "");
        }

        public void TraceApi(string componentName, string method, TimeSpan timeSpan, string properties)
        {
            var message = string.Concat("Component:", componentName, " ;Method:", method, " ;Timespan:",
                timeSpan.ToString(), " ;Properties:", properties);
            Trace.TraceInformation(message);
        }

        public void TraceApi(string componentName, string method, TimeSpan timeSpan, string fmt, params object[] vars)
        {
            TraceApi(componentName, method, timeSpan, string.Format(fmt, vars));
        }


        public static string FormatExceptionMessage(Exception exception, string fmt, object[] vars)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format(fmt, vars));
            sb.Append(" Exception: ");
            sb.Append(exception);
            return sb.ToString();
        }
    }
}