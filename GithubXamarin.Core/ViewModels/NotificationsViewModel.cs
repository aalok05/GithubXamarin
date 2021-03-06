﻿using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using GithubXamarin.Core.Contracts.Service;
using GithubXamarin.Core.Contracts.ViewModel;
using GithubXamarin.Core.Messages;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using Octokit;

namespace GithubXamarin.Core.ViewModels
{
    public class NotificationsViewModel : BaseViewModel, INotificationsViewModel
    {
        #region Properties and Commands

        private readonly INotificationDataService _notificationDataService;

        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get { return _notifications; }
            set
            {
                _notifications = value;
                RaisePropertyChanged(() => Notifications);
            }
        }

        private Notification _markedNotification;
        public Notification MarkedNotification
        {
            get { return _markedNotification; }
            set
            {
                _markedNotification = value;
                RaisePropertyChanged(() => MarkedNotification);
            }
        }

        private ICommand _markNotificationAsReadCommand;
        public ICommand MarkNotificationAsReadCommand
        {
            get
            {
                _markNotificationAsReadCommand = _markNotificationAsReadCommand ?? new MvxCommand<Notification>(notification => MarkNotificationAsRead(notification));
                return _markNotificationAsReadCommand;
            }
        }

        private ICommand _markAllNotificationsAsReadCommand;
        public ICommand MarkAllNotificationsAsReadCommand
        {
            get
            {
                _markAllNotificationsAsReadCommand = _markAllNotificationsAsReadCommand ??
                                                 new MvxAsyncCommand(async () => await MarkAllNotificationsAsRead());
                return _markAllNotificationsAsReadCommand;
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new MvxAsyncCommand(async () => await Refresh());
                return _refreshCommand;
            }
        }

        private long? _repositoryId;
        #endregion


        public NotificationsViewModel(IGithubClientService githubClientService, INotificationDataService notificationDataService, IMvxMessenger messenger, IDialogService dialogService) : base(githubClientService, messenger, dialogService)
        {
            _notificationDataService = notificationDataService;
        }

        public async void Init(long? repositoryId = null)
        {
            _repositoryId = repositoryId;
            await Refresh();
        }

        public async Task Refresh()
        {
            if (!IsInternetAvailable())
            {
                await DialogService.ShowDialogASync("What is better ? To be born good or to overcome your evil nature through great effort ?", "No Internet Connection!");
                return;
            }
            Messenger.Publish(new LoadingStatusMessage(this) { IsLoadingIndicatorActive = true });
            try
            {
                if (_repositoryId.HasValue)
                {
                    Messenger.Publish(new AppBarHeaderChangeMessage(this) { HeaderTitle = "Notifications" });
                    Notifications = await _notificationDataService.GetAllNotificationsForRepository(_repositoryId.Value,
                        GithubClientService.GetAuthorizedGithubClient());
                }
                else
                {
                    Messenger.Publish(new AppBarHeaderChangeMessage(this) { HeaderTitle = "Your Notifications" });
                    Notifications =
                        await _notificationDataService.GetAllNotificationsForCurrentUser(
                            GithubClientService.GetAuthorizedGithubClient());
                }
            }
            catch (HttpRequestException)
            {
                await DialogService.ShowDialogASync("The internet seems to be working but the code threw an HttpRequestException. Try again.", "Hmm, this is weird!");
            }
            Messenger.Publish(new LoadingStatusMessage(this) { IsLoadingIndicatorActive = false });
        }

        private async Task MarkNotificationAsRead(Notification notification)
        {
            if (MarkedNotification == null)
                return;
            var notificationsClient = new NotificationsClient(new ApiConnection(GithubClientService.GetAuthorizedGithubClient().Connection));

            await notificationsClient.MarkAsRead(int.Parse(MarkedNotification.Id));
        }

        private async Task MarkAllNotificationsAsRead()
        {
            var notificationsClient = new NotificationsClient(new ApiConnection(GithubClientService.GetAuthorizedGithubClient().Connection));

            await notificationsClient.MarkAsRead();
            await Refresh();
        }
    }
}
