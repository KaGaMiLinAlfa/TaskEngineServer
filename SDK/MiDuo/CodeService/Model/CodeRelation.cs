using System;
using System.Collections.Generic;
using System.Text;

namespace SDK.MiDuo.CodeService.Model
{
    public class CodeRelation
    {
        public string MemberLogin { get; set; }
        public string Batch { get; set; }
        public string Segment { get; set; }
        public bool BigSerialIsNull { get; set; }
        public int StorageState { get; set; } = -1;
    }
}
