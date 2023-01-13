namespace Strem.Plugins.Analytics.Viewer.Models;

public record InteractionMetric(DateTime Date, int Viewers, int ChatCount, int UsersJoined, int UsersLeft);
