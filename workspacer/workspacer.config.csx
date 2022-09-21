// Production
#r "C:\Program Files\workspacer\workspacer.Shared.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.Bar\workspacer.Bar.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.Gap\workspacer.Gap.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.FocusBorder\workspacer.FocusBorder.dll"

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using workspacer;
using workspacer.Bar;
using workspacer.Bar.Widgets;
using workspacer.Gap;
using workspacer.ActionMenu;
using workspacer.FocusIndicator;
using workspacer.FocusBorder;

return new Action<IConfigContext>((IConfigContext context) =>
{
    context.ToggleConsoleWindow(); //disable console on startup

    /* Variables */
    var fontSize = 10;
    var barHeight = 15;
    var fontName = "mononoki";
    var background = new Color(22, 27, 36);

    /* Config */
    context.CanMinimizeWindows = true;

    /* Gap */
    var gap = barHeight;
    var gapPlugin = context.AddGap(new GapPluginConfig() { InnerGap = gap, OuterGap = gap / 2, Delta = gap / 2 });

    /* Bar */
    context.AddBar(new BarPluginConfig()
    {
        FontSize = fontSize,
        BarHeight = barHeight,
        FontName = fontName,
        DefaultWidgetBackground = background,
        LeftWidgets = () => new IBarWidget[]
        {
            new WorkspaceWidget(), new TextWidget("|"), new TitleWidget() {
                MonitorHasFocusColor = Color.Red,
                IsShortTitle = true,
                NoWindowMessage = ""
            }
        },
        RightWidgets = () => new IBarWidget[]
        {
            new CpuPerformanceWidget() {StringFormat = "⚙️" + "{0}%"  },
            new TextWidget("|"),
            new MemoryPerformanceWidget(){StringFormat = "🗃️" + "{0}%"  },
            new TextWidget("|"),
             new TextWidget("🔋"), new BatteryWidget(),
             new TextWidget("|"),
            new TimeWidget(1000, "HH:mm dd/MM/yyyy"),
/*             new TextWidget("|"),
            new ActiveLayoutWidget(), */
        }
    });

    /* Bar focus indicator */
    context.AddFocusBorder(new FocusBorderPluginConfig()
    {
        BorderColor = new Color(101, 155, 172),
        BorderSize = 5,
        Opacity = 0.8

    });
/*     context.HandleDisplaySettingsChanged => () {
                    SaveState();
            var response = new LauncherResponse()
            {
                Action = LauncherAction.RestartWithMessage,
                Message = "A display settings change has been detected, which has automatically disabled workspacer. Press 'restart' when ready.",
            };
            SendResponse(response);

            CleanupAndExit();
    }; */

    /* Default layouts */
    Func<ILayoutEngine[]> defaultLayouts = () => new ILayoutEngine[]
    {
        new TallLayoutEngine(),
        new VertLayoutEngine(),
        new HorzLayoutEngine(),
        new FullLayoutEngine(),
    };

    context.DefaultLayouts = defaultLayouts;

    /* Workspaces */
    // Array of workspace names and their layouts
    (string, ILayoutEngine[])[] workspaces =
    {
        ("🌎Web", defaultLayouts()),
        ("💬Chat", defaultLayouts()),
        ("🚀Launcher",defaultLayouts()),
        ("🎮Games", defaultLayouts()),
        ("⌨️Coding", defaultLayouts()),
        ("🎸Music", defaultLayouts()),
    };
    

    foreach ((string name, ILayoutEngine[] layouts) in workspaces)
    {
        context.WorkspaceContainer.CreateWorkspace(name, layouts);
    }

    /* Filters */
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("1Password.exe"));
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("pinentry.exe"));
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("AltSnap.exe"));
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("PowerToys.PowerLauncher.exe"));

    // The following filter means that Edge will now open on the correct display
    context.WindowRouter.AddFilter((window) => !window.Class.Equals("ShellTrayWnd"));
    context.WindowRouter.AddFilter((window) => !window.Class.Equals("OperationStatusWindow")); //Explorer File copy
    context.WindowRouter.AddFilter((window) => !window.Class.Equals("#32770")); //File Selector


    /* Action menu */
    var actionMenu = context.AddActionMenu(new ActionMenuPluginConfig()
    {
        RegisterKeybind = false,
        MenuHeight = barHeight,
        FontSize = fontSize,
        FontName = fontName,
        Background = background,
    });

    /* Action menu builder */
    Func<ActionMenuItemBuilder> createActionMenuBuilder = () =>
    {
        var menuBuilder = actionMenu.Create();

        // Switch to workspace
        menuBuilder.AddMenu("switch", () =>
        {
            var workspaceMenu = actionMenu.Create();
            var monitor = context.MonitorContainer.FocusedMonitor;
            var workspaces = context.WorkspaceContainer.GetWorkspaces(monitor);

            Func<int, Action> createChildMenu = (workspaceIndex) => () =>
            {
                context.Workspaces.SwitchMonitorToWorkspace(monitor.Index, workspaceIndex);
            };

            int workspaceIndex = 0;
            foreach (var workspace in workspaces)
            {
                workspaceMenu.Add(workspace.Name, createChildMenu(workspaceIndex));
                workspaceIndex++;
            }

            return workspaceMenu;
        });

        // Move window to workspace
        menuBuilder.AddMenu("move", () =>
        {
            var moveMenu = actionMenu.Create();
            var focusedWorkspace = context.Workspaces.FocusedWorkspace;

            var workspaces = context.WorkspaceContainer.GetWorkspaces(focusedWorkspace).ToArray();
            Func<int, Action> createChildMenu = (index) => () => { context.Workspaces.MoveFocusedWindowToWorkspace(index); };

            for (int i = 0; i < workspaces.Length; i++)
            {
                moveMenu.Add(workspaces[i].Name, createChildMenu(i));
            }

            return moveMenu;
        });

        // Rename workspace
        menuBuilder.AddFreeForm("rename", (name) =>
        {
            context.Workspaces.FocusedWorkspace.Name = name;
        });

        // Create workspace
        menuBuilder.AddFreeForm("create workspace", (name) =>
        {
            context.WorkspaceContainer.CreateWorkspace(name);
        });

        // Delete focused workspace
        menuBuilder.Add("close", () =>
        {
            context.WorkspaceContainer.RemoveWorkspace(context.Workspaces.FocusedWorkspace);
        });

        // Workspacer
        menuBuilder.Add("toggle keybind helper", () => context.Keybinds.ShowKeybindDialog());
        menuBuilder.Add("toggle enabled", () => context.Enabled = !context.Enabled);
        menuBuilder.Add("restart", () => context.Restart());
        menuBuilder.Add("quit", () => context.Quit());

        return menuBuilder;
    };
    var actionMenuBuilder = createActionMenuBuilder();

    /* Keybindings */
    Action setKeybindings = () =>
    {
        KeyModifiers winShift = KeyModifiers.Win | KeyModifiers.Shift;
        KeyModifiers winCtrl = KeyModifiers.Win | KeyModifiers.Control;
        KeyModifiers win = KeyModifiers.Win;

        IKeybindManager manager = context.Keybinds;

        var workspaces = context.Workspaces;

        manager.UnsubscribeAll();
        manager.Subscribe(MouseEvent.LButtonDown, () => workspaces.SwitchFocusedMonitorToMouseLocation());

        foreach (IWorkspace workspace in context.WorkspaceContainer.GetWorkspaces(context.MonitorContainer.FocusedMonitor))
        {
            int i = context.WorkspaceContainer.GetWorkspaceIndex(workspace);
            manager.Subscribe(win, (Keys)(49 + i), () => workspaces.SwitchToWorkspace(workspace), "Switch to Workspace " + (i+1));
            manager.Subscribe(winShift, (Keys)(49 + i), () => workspaces.MoveFocusedWindowToWorkspace(i), "Switch to Workspace " + (i+1));
        }

        manager.Subscribe(win, Keys.Return, () => Process.Start ("wt.exe"),"Open terminal");
        manager.Subscribe(win, Keys.W, () => Process.Start ("chrome.exe"),"Open browser");

        // Left, Right keys
        manager.Subscribe(winCtrl, Keys.Left, () => workspaces.SwitchToPreviousWorkspace(), "switch to previous workspace");
        manager.Subscribe(winCtrl, Keys.Right, () => workspaces.SwitchToNextWorkspace(), "switch to next workspace");

        manager.Subscribe(winShift, Keys.Left, () => workspaces.MoveFocusedWindowToPreviousMonitor(), "move focused window to previous monitor");
        manager.Subscribe(winShift, Keys.Right, () => workspaces.MoveFocusedWindowToNextMonitor(), "move focused window to next monitor");

        // H, L keys
        manager.Subscribe(winShift, Keys.H, () => workspaces.FocusedWorkspace.ShrinkPrimaryArea(), "shrink primary area");
        manager.Subscribe(winShift, Keys.L, () => workspaces.FocusedWorkspace.ExpandPrimaryArea(), "expand primary area");

        manager.Subscribe(winCtrl, Keys.H, () => workspaces.FocusedWorkspace.DecrementNumberOfPrimaryWindows(), "decrement number of primary windows");
        manager.Subscribe(winCtrl, Keys.L, () => workspaces.FocusedWorkspace.IncrementNumberOfPrimaryWindows(), "increment number of primary windows");

        // K, J keys
        manager.Subscribe(winShift, Keys.K, () => workspaces.FocusedWorkspace.SwapFocusAndNextWindow(), "swap focus and next window");
        manager.Subscribe(winShift, Keys.J, () => workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow(), "swap focus and previous window");

        manager.Subscribe(win, Keys.K, () => workspaces.FocusedWorkspace.FocusNextWindow(), "focus next window");
        manager.Subscribe(win, Keys.J, () => workspaces.FocusedWorkspace.FocusPreviousWindow(), "focus previous window");

        // Add, Subtract keys
        manager.Subscribe(winCtrl, Keys.Add, () => gapPlugin.IncrementInnerGap(), "increment inner gap");
        manager.Subscribe(winCtrl, Keys.Subtract, () => gapPlugin.DecrementInnerGap(), "decrement inner gap");

        manager.Subscribe(winShift, Keys.Add, () => gapPlugin.IncrementOuterGap(), "increment outer gap");
        manager.Subscribe(winShift, Keys.Subtract, () => gapPlugin.DecrementOuterGap(), "decrement outer gap");

        // Other shortcuts
        manager.Subscribe(winCtrl, Keys.P, () => actionMenu.ShowMenu(actionMenuBuilder), "show menu");
        manager.Subscribe(winCtrl, Keys.R, () => context.Restart(), "show menu");
        
        manager.Subscribe(winShift, Keys.Escape, () => context.Enabled = !context.Enabled, "toggle enabled/disabled");
        manager.Subscribe(winShift, Keys.I, () => context.ToggleConsoleWindow(), "toggle console window");
    };
    setKeybindings();
});
