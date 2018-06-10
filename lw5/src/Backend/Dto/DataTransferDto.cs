using System;
using System.Runtime.Serialization;

namespace Backend.Dto
{
    [DataContract] 
    public class DataTransferDto
    {
        [DataMember] 
        public string Data {get; set;}
    }
}