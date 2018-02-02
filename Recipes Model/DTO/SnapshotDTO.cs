using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.DTO
{
    public class SnapshotDTO
    {
        public string Path { get; set; }
        public DateTime FullDate { get; set; }

        public SnapshotDTO(string path)
        {
            Path = path;

            var pathSplit = path.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            var dateString = pathSplit[1];
            var timeString = pathSplit[2].Remove(pathSplit[2].LastIndexOf('.'));

            var dateSplit = dateString.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var timeSplit = timeString.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            var day = int.Parse(dateSplit[0]);
            var month = int.Parse(dateSplit[1]);
            var year = int.Parse(dateSplit[2]);

            var hour = int.Parse(timeSplit[0]);
            var minutes = int.Parse(timeSplit[1]);
            var seconds = int.Parse(timeSplit[2]);

            FullDate = new DateTime(year, month, day, hour, minutes, seconds);
        }
    }
}
