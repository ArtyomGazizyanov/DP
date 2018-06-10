using System;
using System.Runtime.Serialization;

namespace Backend.Dto
{
    [DataContract] 
    public class StatisticsDto
    {
        [DataMember] 
        public string TextNum {get; set;}
        
        [DataMember] 
        public string HighRankPart {get; set;}
        
        [DataMember] 
        public string AvgRank {get; set;}
    }
}