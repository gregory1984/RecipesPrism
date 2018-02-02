using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Data;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Unity;
using Recipes_Prism.Events;
using Recipes_Prism.Events.Payloads;
using Recipes_Prism.Events.Pagination;
using Recipes_Prism.Helpers;
using Recipes_Prism.Enums;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;
using Recipes_Model.DTO.Searching;
namespace Recipes_Prism.ViewModels
{
    public class BackupWindowViewModel : BindableBase
    {
        private string snapshotName = "";
        public string SnapshotName
        {
            get => snapshotName;
            set
            {
                SetProperty(ref snapshotName, value);
                SnapshotsView.Refresh();
            }
        }

        private ICollectionView snapshotsView;
        public ICollectionView SnapshotsView
        {
            get => snapshotsView;
            set => SetProperty(ref snapshotsView, value);
        }

        private SnapshotViewModel selectedSnapshot;
        public SnapshotViewModel SelectedSnapshot
        {
            get => selectedSnapshot;
            set
            {
                SetProperty(ref selectedSnapshot, value);
                Recover.RaiseCanExecuteChanged();
                Delete.RaiseCanExecuteChanged();
            }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IBackupService backupService;

        public BackupWindowViewModel(IEventAggregator eventAggregator, IBackupService backupService)
        {
            this.eventAggregator = eventAggregator;
            this.backupService = backupService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() => SetSnapshots());
                backupService.BackupFinishedAction += (snapshotFile) =>
                {
                    SnapshotCreationFinishedAction?.Invoke(snapshotFile);
                    SetSnapshots();
                };

                backupService.RecoveringFinishedAction += (snapshotFile) =>
                {
                    SnapshotRecoveringFinishedAction?.Invoke(snapshotFile.Substring(snapshotFile.LastIndexOf('\\') + 1));
                    eventAggregator.GetEvent<RestartApplicationEvent>().Publish();
                };
            }));
        }

        private DelegateCommand backup;
        public DelegateCommand Backup
        {
            get => backup ?? (backup = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    backupService.MakeSnapshot();
                    SetSnapshots();
                });
            }));
        }

        private DelegateCommand recover;
        public DelegateCommand Recover
        {
            get => recover ?? (recover = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)RecoverSelectedSnapshotQuestionAction?.Invoke(SelectedSnapshot.FullDate))
                    {
                        backupService.Recover(SelectedSnapshot.Path);
                    }
                });
            },

            () => SelectedSnapshot != null));
        }

        private DelegateCommand delete;
        public DelegateCommand Delete
        {
            get => delete ?? (delete = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)DeleteSelectedSnapshotQuestionAction?.Invoke(SelectedSnapshot.FullDate))
                    {
                        backupService.DeleteSnapshot(SelectedSnapshot.Path);
                        SetSnapshots();
                    }
                });
            },

            () => SelectedSnapshot != null));
        }

        private DelegateCommand closeWindow;
        public DelegateCommand CloseWindow
        {
            get => closeWindow ?? (closeWindow = new DelegateCommand(() => CloseWindowAction?.Invoke()));
        }

        private void SetSnapshots()
        {
            var snapshotsDTOs = backupService.GetSnapshots();
            var snapshots = new List<SnapshotViewModel>();
            foreach (var dto in snapshotsDTOs)
                snapshots.Add(new SnapshotViewModel(dto, snapshots.Count + 1));

            SnapshotsView = CollectionViewSource.GetDefaultView(snapshots);
            SnapshotsView.Filter = (object item) =>
            {
                var snapshot = item as SnapshotViewModel;
                var date = snapshot.FullDate.ToString("dd-MM-yyyy HH:mm:ss");

                return date.Contains(SnapshotName.ToLower());
            };
        }

        public Func<DateTime, bool> DeleteSelectedSnapshotQuestionAction { get; set; }
        public Func<DateTime, bool> RecoverSelectedSnapshotQuestionAction { get; set; }
        public Action<string> SnapshotCreationFinishedAction { get; set; }
        public Action<string> SnapshotRecoveringFinishedAction { get; set; }
        public Action CloseWindowAction { get; set; }
    }
}
