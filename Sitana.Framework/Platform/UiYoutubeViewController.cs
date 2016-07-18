using System.Collections.Generic;
using Sitana.Framework.Ui.Core;
using UIKit;

namespace Sitana.Framework
{
    public class UiYoutubeViewController: UIViewController
	{
        string _videoId;

		public UiYoutubeViewController(string videoId)
		{
            _videoId = videoId;
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			View.BackgroundColor = UIColor.Black;
            Platform.GestureRecognizer.Disabled = true;
		}

        public override void ViewDidDisappear (bool animated)
        {
            base.ViewDidDisappear (animated);
            Platform.GestureRecognizer.Disabled = false;
        }

		public override void ViewDidLoad()
		{
            base.ViewDidLoad();

			var webViewFrame = View.Frame;

			// Perform any additional setup after loading the view, typically from a nib.
			var player = new YouTubePlayer(webViewFrame);

            player.PlayerView.BackgroundColor = UIColor.Black;

            View.Add(player.PlayerView);

            player.PlayerViewReady += (o, e) => 
            {
                player.SetPlaybackQuality(PlaybackQuality.HD1080);
                player.PlayVideo();
            };

            player.OnClose += (o, e) => 
            {
                player.StopVideo ();
                player.ClearVideo();

                player.PlayerView.RemoveFromSuperview();

                DismissViewController(true, null);
            };

            var parameters = new Dictionary<string, object> ();
            parameters.Add ("fs", 0);

            parameters.Add ("showinfo", 0);

            player.LoadWithVideoId(_videoId, parameters);
		}
	}
}

