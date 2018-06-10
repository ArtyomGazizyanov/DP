using System;
using System.Runtime.Serialization;

namespace Frontend.Dto
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