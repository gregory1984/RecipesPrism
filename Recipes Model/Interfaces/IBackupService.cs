using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipes_Model.DTO;

namespace Recipes_Model.Interfaces
{
    public interface IBackupService
    {
        string SnapshotsPath { get; }
        Action<string> BackupFinishedAction { get; set; }
        Action<string> RecoveringFinishedAction { get; set; }

        IList<SnapshotDTO> GetSnapshots();

        void DeleteSnapshot(string path);
        void MakeSnapshot();
        void Recover(string path);
    }
}
