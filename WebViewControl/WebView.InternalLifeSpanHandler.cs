using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Handlers;

namespace WebViewControl {

    partial class WebView {

        private class InternalLifeSpanHandler : LifeSpanHandler {

            private WebView OwnerWebView { get; }

            public InternalLifeSpanHandler(WebView webView) {
                OwnerWebView = webView;
            }

            //protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess) {     
            //   if (UrlHelper.IsChromeInternalUrl(targetUrl)) {
            //        return false;
            //    }

            //    if (Uri.IsWellFormedUriString(targetUrl, UriKind.RelativeOrAbsolute)) {
            //        var uri = new Uri(targetUrl);
            //        if (!uri.IsAbsoluteUri) {
            //            // turning relative urls into full path to avoid that someone runs custom command lines
            //            targetUrl = new Uri(new Uri(frame.Url), uri).AbsoluteUri;
            //        }
            //    } else {
            //        return false; // if the url is not well formed let's use the browser to handle the things
            //    }

            //    try {
            //        var popupOpening = OwnerWebView.PopupOpening;
            //        if (popupOpening != null) {
            //            popupOpening(targetUrl);
            //        } else {
            //            UrlHelper.OpenInExternalBrowser(targetUrl);
            //        }
            //    } catch {
            //        // if we can't handle the command line let's continue the normal request with the popup
            //        // with this, will not blow in the users face
            //        return false;
            //    }

            //    return true;
            //}

            protected override bool OnBeforePopup(
               CefBrowser browser,
               CefFrame frame,
               string targetUrl,
               string targetFrameName,
               CefWindowOpenDisposition targetDisposition,
               bool userGesture,
               CefPopupFeatures popupFeatures,
               CefWindowInfo windowInfo,
               ref CefClient client,
               CefBrowserSettings settings,
               ref CefDictionaryValue extraInfo,
               ref bool noJavascriptAccess) {
                // 处理 Chrome 内部 URL
                if (UrlHelper.IsChromeInternalUrl(targetUrl)) {
                    return false;
                }
    
                // 处理相对 URL
                if (Uri.IsWellFormedUriString(targetUrl, UriKind.RelativeOrAbsolute)) {
                    var uri = new Uri(targetUrl);
                    if (!uri.IsAbsoluteUri) {
                        // 将相对 URL 转换为绝对路径，以避免恶意命令
                        targetUrl = new Uri(new Uri(frame.Url), uri).AbsoluteUri;
                    }
                } else {
                    return false; // 如果 URL 无效，交给默认的浏览器处理
                }
                // 对于 Linux 特定的处理，检查是否需要修改窗口信息
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    // Linux 上处理，避免创建新窗口
                    // 确保不创建新的 `CefWindowInfo` 或者设定正确的窗口样式
                    windowInfo.SetAsPopup(IntPtr.Zero, targetFrameName);  // 设定为弹出窗口（可能在 Linux 上不创建实际的新窗口）
               
                }
                // 判断是否为新窗口打开
                if (targetDisposition == CefWindowOpenDisposition.NewWindow || OwnerWebView.PopupOpening==null) {
                    // 阻止新窗口打开，加载到当前浏览器窗口
                    try {

               
                        // 在当前浏览器窗口加载 URL
                        browser.GetMainFrame().LoadUrl(targetUrl);
                        return true; // 阻止新窗口打开
                    } catch (Exception ex) {
                        // 如果加载失败，仍然交给默认浏览器处理
                        Console.WriteLine($"Error loading URL in current window: {ex.Message}");
                        UrlHelper.OpenInExternalBrowser(targetUrl);
                        return false;
                    }
                } 

                // 其他情况，交给原有的处理逻辑
                try {
                    var popupOpening = OwnerWebView.PopupOpening;
                    if (popupOpening != null) {
                        popupOpening(targetUrl);
                    } else {
                        UrlHelper.OpenInExternalBrowser(targetUrl);
                    }
                } catch {
                    // 如果无法处理，交给默认浏览器处理
                    return false;
                }

                return true;
            }

        }
    }
}
