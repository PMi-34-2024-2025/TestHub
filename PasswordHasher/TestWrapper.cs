using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE.DesktopApplication.TestHub.BLL
{
    public class TestWrapper
    {
        public string title;
        public string description;
        public int id;
        public string topic;
        public string role;
        public string status;
        public TestWrapper(string title, string description, int id, string role = "student", string status = "pending", string topic = "All")
        {
            this.title = title;
            this.description = description;
            this.id = id;
            this.topic = topic;
            this.role = role;
            this.status = status;
        }
        public override bool Equals(object? obj)
        {
            if (obj.GetType() != typeof(TestWrapper)) return false;
            return id == ((TestWrapper)obj).id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
