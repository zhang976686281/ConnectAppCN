using System.Collections.Generic;
using ConnectApp.models;
using ConnectApp.plugins;
using ConnectApp.screens;
using RSG;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace ConnectApp.canvas {
    internal static class MainNavigatorRoutes {
        public const string Root = "/";
        public const string Search = "/search";
        public const string EventDetail = "/event-detail";
        public const string ArticleDetail = "/article-detail";
        public const string Setting = "/setting";
        public const string MyEvent = "/my-event";
        public const string History = "/history";
        public const string Login = "/login";
        public const string BindUnity = "/bind-unity";
        public const string VideoPlayer = "/video-player";
        public const string Report = "/report";
        public const string AboutUs = "/aboutUs";
        public const string WebView = "/web-view";
    }

    internal class Router : StatelessWidget {
        private static readonly GlobalKey globalKey = GlobalKey.key("main-router");
        public static NavigatorState navigator => globalKey.currentState as NavigatorState;

        private static Dictionary<string, WidgetBuilder> mainRoutes => new Dictionary<string, WidgetBuilder> {
            {MainNavigatorRoutes.Root, context => new MainScreen()},
            {MainNavigatorRoutes.Search, context => new SearchScreenConnector()},
            {MainNavigatorRoutes.ArticleDetail, context => new ArticleDetailScreenConnector("")},
            {MainNavigatorRoutes.Setting, context => new SettingScreenConnector()},
            {MainNavigatorRoutes.MyEvent, context => new MyEventsScreenConnector()},
            {MainNavigatorRoutes.History, context => new HistoryScreenConnector()},
            {MainNavigatorRoutes.Login, context => new LoginScreen()},
            {MainNavigatorRoutes.BindUnity, context => new BindUnityScreenConnector(FromPage.setting)},
            {MainNavigatorRoutes.Report, context => new ReportScreenConnector("", ReportType.article)},
            {MainNavigatorRoutes.AboutUs, context => new AboutUsScreenConnector()},
            {MainNavigatorRoutes.WebView, context => new WebViewScreen()}
        };

        private static Dictionary<string, WidgetBuilder> fullScreenRoutes => new Dictionary<string, WidgetBuilder> {
            {MainNavigatorRoutes.Search, context => new SearchScreenConnector()},
            {MainNavigatorRoutes.Login, context => new LoginScreen()}
        };

        public override Widget build(BuildContext context) {
            JPushPlugin.context = context;
            return new WillPopScope(
                onWillPop: () =>
                {
                    var promise = new Promise<bool>();
                    if (LoginScreen.navigator?.canPop() ?? false) {
                        LoginScreen.navigator.pop();
                        promise.Resolve(false);
                    } else if (navigator.canPop()) {
                        navigator.pop();
                        promise.Resolve(false);
                    } else {
                        promise.Resolve(true);
                    }
                    return promise;
                },
                child:new Navigator(
                    globalKey,
                    onGenerateRoute: settings => {
                        return new PageRouteBuilder(
                            settings,
                            (context1, animation, secondaryAnimation) => mainRoutes[settings.name](context1),
                            (context1, animation, secondaryAnimation, child) => {
                                if (fullScreenRoutes.ContainsKey(settings.name))
                                    return new ModalPageTransition(
                                        routeAnimation: animation,
                                        child: child
                                    );
                                return new PushPageTransition(
                                    routeAnimation: animation,
                                    child: child
                                );
                            }
                        );
                    }
                )
           ); 
        }
    }

    internal class PushPageTransition : StatelessWidget {
        internal PushPageTransition(
            Key key = null,
            Animation<float> routeAnimation = null, // The route's linear 0.0 - 1.0 animation.
            Widget child = null
        ) : base(key) {
            _positionAnimation = _leftPushTween.chain(_fastOutSlowInTween).animate(routeAnimation);
            this.child = child;
        }

        private readonly Tween<Offset> _leftPushTween = new OffsetTween(
            new Offset(1.0f, 0.0f),
            Offset.zero
        );

        private readonly Animatable<float> _fastOutSlowInTween = new CurveTween(Curves.fastOutSlowIn);
        private readonly Animation<Offset> _positionAnimation;
        private readonly Widget child;

        public override Widget build(BuildContext context) {
            return new SlideTransition(
                position: _positionAnimation,
                child: child
            );
        }
    }

    internal class ModalPageTransition : StatelessWidget {
        internal ModalPageTransition(
            Key key = null,
            Animation<float> routeAnimation = null, // The route's linear 0.0 - 1.0 animation.
            Widget child = null
        ) : base(key) {
            _positionAnimation = _bottomUpTween.chain(_fastOutSlowInTween).animate(routeAnimation);
            _opacityAnimation = _easeInTween.animate(routeAnimation);
            this.child = child;
        }

        private readonly Tween<Offset> _bottomUpTween = new OffsetTween(
            new Offset(0.0f, 1.0f),
            Offset.zero
        );

        private readonly Animatable<float> _fastOutSlowInTween = new CurveTween(Curves.fastOutSlowIn);
        private readonly Animatable<float> _easeInTween = new CurveTween(Curves.easeIn);

        private readonly Animation<Offset> _positionAnimation;
        private readonly Animation<float> _opacityAnimation;
        private readonly Widget child;

        public override Widget build(BuildContext context) {
            return new SlideTransition(
                position: _positionAnimation,
                child: child
            );
        }
    }
}