using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonetArxLearn.EmployeeSample
{
    class Employee
    {
        public string Name { get; set; } = "Earnest Shackleton";
        public double Salary { get; set; } = 10000;
        public string Division { get; set; } = "Sales";

        public override string ToString()
        {
            return $"Name: {Name} \t Salary: {Salary}\t Division: {Division}";
        }
    }
}
