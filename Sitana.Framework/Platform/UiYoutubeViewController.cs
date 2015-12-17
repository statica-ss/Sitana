using System;
using UIKit;
using Foundation;

namespace Sitana.Framework
{
	public class UiYoutubeViewController: UIViewController
	{
		string _html;

//		class YtWebViewDelegate: UIWebViewDelegate
//		{
//			public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
//			{
//				if (navigationType == UIWebViewNavigationType.LinkClicked)
//				{
//					return false;
//				}
//				return true;
//			}
//		}

		public UiYoutubeViewController(string movieId)
		{
			string url = string.Format("http://www.youtube.com/embed/{0}?autoplay=1", movieId);

			string youtubeFormat = 
				"<html><body bgcolor=\"black\" style=\"margin:0\">" +
				"<iframe width=\"100%\" height=\"100%\" src=\"{0}\" frameborder=\"0\" allowfullscreen></iframe>" +
				"</body></html>";

			_html = string.Format(youtubeFormat, url);
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			View.BackgroundColor = UIColor.Black;
		}

		public override void ViewDidLoad()
		{
			const int toolbarHeight = 44;

			base.ViewDidLoad();

			var webViewFrame = this.View.Frame;
			var toolbarFrame = this.View.Frame;

			toolbarFrame.Height = toolbarHeight;

			webViewFrame.Y = toolbarHeight;
			webViewFrame.Height -= toolbarHeight;

			// Perform any additional setup after loading the view, typically from a nib.
			UIWebView webView = new UIWebView (webViewFrame);
//			YtWebViewDelegate del = new YtWebViewDelegate();
//
//			webView.Delegate = del;
			webView.BackgroundColor = UIColor.Black;

			var doneBarButton = new UIBarButtonItem("Gotowe", UIBarButtonItemStyle.Done, (o, e) =>
			{
				webView.LoadHtmlString("<html></html>", null);
				webView.RemoveFromSuperview();
				DismissModalViewController(true);
			});

			UIBarButtonItem[] items = new UIBarButtonItem[] 
			{
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				doneBarButton
			};

			UIToolbar toolbar = new UIToolbar(toolbarFrame);
			toolbar.SetItems(items, false);

			toolbar.BarStyle = UIBarStyle.Black;

			webView.Init();
			webView.LoadHtmlString(_html, null);

			View.Add(webView);
			View.Add(toolbar);


		}
	}
}

