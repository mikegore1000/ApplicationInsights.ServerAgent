using System;

namespace ApplicationInsights.ServerAgent
{
    internal static class Guard
    {
        internal static void IsNotNullOrEmpty(string paramName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
            }
        }

        internal static void IsNotNull(string paramName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null");
            }
        }
    }
}
