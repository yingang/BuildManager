//-----------------------------------------------------------------------
// <copyright file="TargetBranchViewModel.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using TfsBuildManager.Repository;

    public class TargetBranchViewModel : ViewModelBase
    {
        private readonly string originalName;
        private string newName;
        private TargetBranch selectedBranch;

        public TargetBranchViewModel(string originalName, IEnumerable<Branch> targets)
        {
            this.originalName = originalName;
            this.TargetBranches = new ObservableCollection<TargetBranch>();
            foreach (var t in targets)
            {
                this.TargetBranches.Add(new TargetBranch { Branch = t, Path = t.ServerPath });
            }
        }

        public ObservableCollection<TargetBranch> TargetBranches { get; private set; }
        
        public string NewName
        {
            get
            {
                return this.newName;
            }

            set
            {
                this.newName = value;
                this.NotifyPropertyChanged("NewName");
                this.NotifyPropertyChanged("IsEnabled");
            }
        }

        public TargetBranch SelectedBranch 
        {
            get
            {
                return this.selectedBranch;
            }

            set 
            {
                var old = this.selectedBranch;
                this.selectedBranch = value;
                SetNewName();
                this.NotifyPropertyChanged("NewName");
                if (value != old)
                {
                    this.NotifyPropertyChanged("IsEnabled");
                }
            }
        }

        public bool IsEnabled 
        {
            get
            {
                return this.SelectedBranch != null && !string.IsNullOrEmpty(this.NewName);
            }
        }

        public string SetNewName()
        {

            string sourceType = this.originalName.Split('_')[0];
            string sourceBranchName = this.originalName.Split('_')[1];
            string targetBranchPath = this.selectedBranch.Path;
            string targetBranchName = Path.GetFileName(targetBranchPath);

            if (sourceType == "Unity")
            {
                SetMainBranch(sourceType,targetBranchPath, targetBranchName);
            }
            else if (sourceType == "BRANCHES" || sourceType == "Release")
            {
                SetOtherBranch(sourceType, sourceBranchName, targetBranchPath, targetBranchName);
            }
            else
            {
                this.NewName = this.originalName + "." + targetBranchName;
            }
            return this.NewName;
        }

        public string SetMainBranch(string sourceBranchType, string targetBranchPath, string targetBranchFile)
        {
            if (targetBranchPath.Contains("$/CT/BRANCHES/Feature"))
            {
                this.NewName = this.originalName.Replace(sourceBranchType, "BRANCHES" + "_" + targetBranchFile);
            }
            else if (targetBranchPath.Contains("$/CT/BRANCHES/Release/"))
            {
                this.NewName = this.originalName.Replace(sourceBranchType, "Release" + "_" + targetBranchFile);
            }
            else if (targetBranchPath.Contains("$/CT/BRANCHES/Users"))
            {
                this.NewName = this.originalName.Replace(sourceBranchType, "User" + "_" + targetBranchFile);
            }
            return this.NewName;
        }

        public string SetOtherBranch(string sourceBranchType, string sourceBranchName, string targetBranchPath, string targetBranchName)
        {
            if (targetBranchPath.Contains("$/CT/BRANCHES/Feature") || targetBranchPath.Contains("$/CT/BRANCHES/Release"))
            {
                this.NewName = this.originalName.Replace(sourceBranchName, targetBranchName);
            }
            else if (targetBranchPath.Contains("$/CT/BRANCHES/Users"))
            {
                this.NewName = this.originalName.Replace(sourceBranchType + "_" + sourceBranchName, "Users" + "_" + targetBranchName);
            }
            return this.NewName;
        }
    }

    public class TargetBranch : ViewModelBase
    {
        public Branch Branch { get; set; }

        public string Path { get; set; }
    }
}
