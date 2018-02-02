using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Recipes_Model.Interfaces;
using Recipes_Model.Database;
using Recipes_Model.DTO;
using Recipes_Model.Helpers;

namespace Recipes_Model.Services
{
    public class BackupService : IBackupService
    {
        public string SnapshotsPath { get; private set; }
        public Action<string> BackupFinishedAction { get; set; }
        public Action<string> RecoveringFinishedAction { get; set; }

        public BackupService()
        {
            SnapshotsPath = "Snapshots\\";
        }

        public IList<SnapshotDTO> GetSnapshots()
        {
            var result = new List<SnapshotDTO>();
            var files = Directory.GetFiles(SnapshotsPath);
            foreach (var f in files.Where(f => f.EndsWith(".sql")))
                result.Add(new SnapshotDTO(f));

            return result.OrderByDescending(r => r.FullDate).ToList();
        }

        public void DeleteSnapshot(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public void MakeSnapshot()
        {
            var config = ConfigurationManager.OpenExeConfiguration(Constants.AssemblyName);
            var connectionString = config.ConnectionStrings.ConnectionStrings["Recipes"].ConnectionString;

            var connectionDataSplit = connectionString.Split(new[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries);

            var password = connectionDataSplit[5];
            var username = connectionDataSplit[3];
            var snapshotFile = "snapshot_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".sql";
            var path = SnapshotsPath + snapshotFile;
            var database = connectionDataSplit[9];
            var dump = $"/C mysqldump -h localhost -p{password} -u {username} --single-transaction -R -r {path} {database}";

            var process = new Process();
            var args = new ProcessStartInfo("cmd.exe", dump);
            process.StartInfo = args;
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => BackupFinishedAction?.Invoke(snapshotFile);
            process.Start();
        }

        public void Recover(string path)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Constants.AssemblyName);
            var connectionString = config.ConnectionStrings.ConnectionStrings["Recipes"].ConnectionString;

            var connectionDataSplit = connectionString.Split(new[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries);

            var password = connectionDataSplit[5];
            var username = connectionDataSplit[3];
            var database = connectionDataSplit[9];
            var recover = $"/C mysql -h localhost -p{password} -u {username} --default-character-set=utf8 Recipes -e \"SOURCE {path.Replace('\\', '/')};\"";

            var process = new Process();
            var args = new ProcessStartInfo("cmd.exe", recover);
            process.StartInfo = args;
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => RecoveringFinishedAction?.Invoke(path);
            process.Start();
        }
    }
}
