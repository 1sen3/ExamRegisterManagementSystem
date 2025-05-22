using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App5.Models
{
    public class ExamInfo
    {
        public int id {  get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public string timestart { get; set; }
        public string timeend { get; set; }
        public string room { get; set; }
        public string status { get; set; }
        public string IdAndName => $"{id} - {name}";
        public void CalculateExamStatus()
        {
            DateTime examDate = DateTime.Parse(date);
            TimeSpan startTime = TimeSpan.Parse(timestart);
            TimeSpan endTime = TimeSpan.Parse(timeend);

            DateTime examStart = examDate.Add(startTime);
            DateTime examEnd = examDate.Add(endTime);

            DateTime now = DateTime.Now;

            if (now > examEnd)
            {
                status="已过期";
            }
            else if (now < examStart)
            {
                status="未开始";
            }
            else
            {
                status="进行中";
            }
        }
    }
}
