﻿namespace OmniXaml.AppServices.NetCore.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OmniXaml.Tests.Classes;
    using Parsers.ProtoParser.SuperProtoParser;
    using Parsers.XamlNodes;
    using Wpf;

    [TestClass]
    public class InflatableFactoryTest
    {
        [TestMethod]
        public void Window()
        {
            var sut = new InflatableTypeFactory(new TypeFactory(), new NetCoreResourceProvider(), new NetCoreTypeToUriLocator())
            {
                Inflatables = new Collection<Type> { typeof(Window) }
            };

            var wiringContext = DummyWiringContext.Create(sut);
            sut.XamlStreamLoaderFactoryMethod = () => new BoostrappableXamlStreamLoader(
                wiringContext,
                new SuperProtoParser(wiringContext),
                new XamlNodesPullParser(wiringContext),
                new DefaultObjectAssemblerFactory(wiringContext));

            var myWindow = sut.Create<MyWindow>();
            Assert.IsInstanceOfType(myWindow, typeof(MyWindow));
            Assert.AreEqual(myWindow.Title, "Hello World :)");
        }

        [TestMethod]
        public void UserControl()
        {
            var sut = new InflatableTypeFactory(new TypeFactory(), new NetCoreResourceProvider(), new NetCoreTypeToUriLocator())
            {
                Inflatables = new Collection<Type> { typeof(Window), typeof(UserControl) }
            };

            var wiringContext = DummyWiringContext.Create(sut);
            sut.XamlStreamLoaderFactoryMethod = () => new BoostrappableXamlStreamLoader(
                wiringContext,
                new SuperProtoParser(wiringContext),
                new XamlNodesPullParser(wiringContext),
                new DefaultObjectAssemblerFactory(wiringContext));

            var myWindow = sut.Create<WindowWithUserControl>();
            Assert.IsInstanceOfType(myWindow, typeof(WindowWithUserControl));
            Assert.AreEqual(myWindow.Title, "Hello World :)");
            Assert.IsInstanceOfType(myWindow.Content, typeof(UserControl));
            Assert.AreEqual("It's-a me, Mario", ((UserControl)myWindow.Content).Property);
        }

        [TestMethod]
        public void UserControlLoadingWithUri()
        {
            var sut = new InflatableTypeFactory(new TypeFactory(), new NetCoreResourceProvider(), new NetCoreTypeToUriLocator())
            {
                Inflatables = new Collection<Type> { typeof(Window), typeof(UserControl) }
            };

            var wiringContext = DummyWiringContext.Create(sut);
            sut.XamlStreamLoaderFactoryMethod = () => new BoostrappableXamlStreamLoader(
                wiringContext,
                new SuperProtoParser(wiringContext),
                new XamlNodesPullParser(wiringContext),
                new DefaultObjectAssemblerFactory(wiringContext));

            var myWindow = (Window)sut.Create(new Uri("WindowWithUserControl.xaml", UriKind.Relative));
            Assert.IsInstanceOfType(myWindow, typeof(WindowWithUserControl));
            Assert.AreEqual(myWindow.Title, "Hello World :)");
            Assert.IsInstanceOfType(myWindow.Content, typeof(UserControl));
            var userControl = ((UserControl)myWindow.Content);
            Assert.AreEqual("It's-a me, Mario", userControl.Property);
            Assert.IsInstanceOfType(userControl.Content, typeof(ChildClass));
        }
    }
}