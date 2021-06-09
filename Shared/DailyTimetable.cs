using System;
using System.Collections.Generic;

namespace Client.Shared
{
    public struct Class
    {
        public int Room { get; set; }
        public bool RoomChanged { get; set; }
        public string Teacher { get; set; }
        public bool TeacherChanged { get; set; }
    }

    public struct TimeChunk
    {
        public DateTime Time { get; set; }
        public string FormattedTime => $"{Time.Hour}:{Time.Minute}";
        public string Name { get; set; }
        public bool ContainsPayload { get; set; }
        public Class Payload { get; set; }
    }

    public struct TimetableResource
    {

    }

    public class DailyTimetableResource
    {
        public List<TimeChunk> Chunks { get; set; } = new List<TimeChunk>();
    }
}
