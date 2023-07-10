using System;

namespace Worker2.EntityModel
{
    public class CodeHandle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public int CodeType { get; set; }

        public int Stats { get; set; }
    }
}
