using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using Chart.Mvc;
using Chart.Mvc.ComplexChart;
using todon.Models;

namespace todon.ViewModels.Statistics
{
    public class StatisticsViewModel
    {
        public Dictionary<int, int> DoneTasks { get; set; }
        public Dictionary<int, string> DoneTasksNames { get; set; }
        public List<int> listDoneDay;
        public List<int> listDoneTasks;
        public int BestDay;
        public int WorstDay;
        public int MonthCount;
        public BarChart Chart { get; set; }


        public StatisticsViewModel(ApplicationDbContext _context)
        {
            DoneTasks = new Dictionary<int, int>();
            DoneTasksNames = new Dictionary<int, string>();
            listDoneTasks = new List<int>();
            listDoneDay = new List<int>();

            for (int i = 0; i < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i + 1);
                listDoneTasks.Add(Done.GetDoneByDate(date, _context).Count);
                listDoneDay.Add(i+1);
                DoneTasks.Add(i + 1, Done.GetDoneByDate(date, _context).Count);
            }

            BestDay = DoneTasks.Max(x => x.Value);
            WorstDay = DoneTasks.Min(x => x.Value);
            MonthCount = DoneTasks.Sum(x => x.Value);
        }

        public StatisticsViewModel(ApplicationDbContext _context, int dayWeek)
        {
            DoneTasks = new Dictionary<int, int>();
            DoneTasksNames = new Dictionary<int, string>();
            listDoneTasks = new List<int>();

            for (int i = dayWeek; i < dayWeek+7; i++)
            {
                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i);//i+1
                listDoneTasks.Add(Done.GetDoneByDate(date, _context).Count);
                DoneTasks.Add(i, Done.GetDoneByDate(date, _context).Count);//i+1
            }
            BestDay = DoneTasks.Max(x => x.Value);
            WorstDay = DoneTasks.Min(x => x.Value);
            MonthCount = DoneTasks.Sum(x => x.Value);
        }

        public StatisticsViewModel()
        {
        }
    }
}

//Chart = new BarChart();
//Chart.ComplexData.Labels.AddRange(new[] { "January", "February", "March", "April", "May", "June", "July" });
//Chart.ComplexData.Datasets.AddRange(new List<ComplexDataset>
//               {
//                  new ComplexDataset
//                      {
//                          Data = new List<double> { 65, 59, 80, 81, 56, 55, 40 },
//                          Label = "My First dataset",
//                          FillColor = "rgba(220,220,220,0.2)",
//                          StrokeColor = "rgba(220,220,220,1)",
//                          PointColor = "rgba(220,220,220,1)",
//                          PointStrokeColor = "#fff",
//                          PointHighlightFill = "#fff",
//                          PointHighlightStroke = "rgba(220,220,220,1)",
//                      },
//                  new ComplexDataset
//                      {
//                          Data = new List<double> { 28, 48, 40, 19, 86, 27, 90 },
//                          Label = "My Second dataset",
//                          FillColor = "rgba(151,187,205,0.2)",
//                          StrokeColor = "rgba(151,187,205,1)",
//                          PointColor = "rgba(151,187,205,1)",
//                          PointStrokeColor = "#fff",
//                          PointHighlightFill = "#fff",
//                          PointHighlightStroke = "rgba(151,187,205,1)",
//                      }
//              });
