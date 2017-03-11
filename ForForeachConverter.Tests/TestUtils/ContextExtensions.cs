﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using System.Linq;
using ASCompletion.Context;
using ASCompletion.Model;
using NSubstitute;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Projects.Haxe;

namespace ForForeachConverter.TestUtils
{
    public static class ContextExtensions
    {
        public static void SetAS3Features(this IASContext context)
        {
            var currentModel = new FileModel {Context = context, Version = 3};
            var asContext = new AS3Context.Context(new AS3Context.AS3Settings());
            BuildClassPath(asContext);
            asContext.CurrentModel = currentModel;
            context.Features.Returns(asContext.Features);
            context.CurrentModel.Returns(currentModel);
            var visibleExternalElements = asContext.GetVisibleExternalElements();
            context.GetVisibleExternalElements().Returns(x => visibleExternalElements);
            context.GetCodeModel(null).ReturnsForAnyArgs(x =>
            {
                var src = x[0] as string;
                return string.IsNullOrEmpty(src) ? null : asContext.GetCodeModel(src);
            });
            context.ResolveType(null, null).ReturnsForAnyArgs(x => asContext.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
        }

        static void BuildClassPath(AS3Context.Context context)
        {
            context.BuildClassPath();
            var intrinsicPath = $"{PathHelper.LibraryDir}{Path.DirectorySeparatorChar}AS3{Path.DirectorySeparatorChar}intrinsic";
            context.Classpath.AddRange(Directory.GetDirectories(intrinsicPath).Select(it => new PathModel(it, context)));
            foreach (var it in context.Classpath)
            {
                if (it.IsVirtual) context.ExploreVirtualPath(it);
                else
                {
                    var path = it.Path;
                    foreach (var searchPattern in context.GetExplorerMask())
                    {
                        foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                        {
                            it.AddFile(ASFileParser.ParseFile(new FileModel(fileName) {Context = context, Version = 3}));
                        }
                    }
                    context.RefreshContextCache(path);
                }
            }
        }

        public static void SetHaxeFeatures(this IASContext context)
        {
            var currentModel = new FileModel {Context = context, Version = 4, haXe = true};
            var haxeContext = new HaXeContext.Context(new HaXeContext.HaXeSettings());
            BuildClassPath(haxeContext);
            haxeContext.CurrentModel = currentModel;
            context.Features.Returns(haxeContext.Features);
            context.CurrentModel.Returns(currentModel);
            var visibleExternalElements = haxeContext.GetVisibleExternalElements();
            context.GetVisibleExternalElements().Returns(x => visibleExternalElements);
            context.GetCodeModel(null).ReturnsForAnyArgs(x =>
            {
                var src = x[0] as string;
                return string.IsNullOrEmpty(src) ? null : haxeContext.GetCodeModel(src);
            });
            context.ResolveType(null, null).ReturnsForAnyArgs(x => haxeContext.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
        }

        static void BuildClassPath(HaXeContext.Context context)
        {
            var platformsFile = Path.Combine("Settings", "Platforms");
            PlatformData.Load(Path.Combine(PathHelper.AppDir, platformsFile));
            PluginBase.CurrentProject = new HaxeProject("haxe") { CurrentSDK = Environment.GetEnvironmentVariable("HAXEPATH") };
            context.BuildClassPath();
            foreach (var it in context.Classpath)
            {
                var path = it.Path;
                foreach (var searchPattern in context.GetExplorerMask())
                {
                    foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                    {
                        it.AddFile(ASFileParser.ParseFile(new FileModel(fileName) { Context = context, haXe = true, Version = 4 }));
                    }
                }
                context.RefreshContextCache(path);
            }
        }
    }
}
