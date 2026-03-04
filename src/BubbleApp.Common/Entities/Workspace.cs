using System;

namespace BubbleApp.Common.Entities
{
    public class Workspace
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string AdminId { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Key-based partition
        public string WorkspaceKey { get; set; } = default!;
        public string WorkspaceKeyHash { get; set; } = default!;

        // --- NEW: appearance (with sensible defaults) ---
        public string BubbleColor { get; set; } = "#5b8def"; // hex string
        public string BubbleText  { get; set; } = "●";       // label shown on the bubble
    }
}