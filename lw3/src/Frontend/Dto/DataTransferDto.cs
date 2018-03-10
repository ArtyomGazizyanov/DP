using System;
using System.Runtime.Serialization;

namespace Frontend.Dto
{
    [DataContract] 
    public class DataTransferDto
    {
        [DataMember] 
        public string Data {get; set;}
    }
}