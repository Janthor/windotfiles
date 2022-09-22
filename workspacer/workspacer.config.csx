// Include plugins and use dependencies
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
    
    //* ******* *//
    //* THEMING *//
    //* ******* *//

    var background      = new Color(39,  46,   62);
    var urgent          = new Color(191, 97,  106);
    var positive        = new Color(163, 190, 140);
    var warning         = new Color(235, 203, 139);
    var foreground      = new Color(129, 161, 193);
    var foreground2     = new Color(180, 142, 173);
    var foreground3     = new Color(136, 192, 208);
    var foreground4     = new Color(229, 233, 240);
    var alt_background  = new Color(86,  106, 116);
    var alt_urgent      = new Color(191, 97,  106);
    var alt_positive    = new Color(163, 190, 140);
    var alt_warning     = new Color(235, 203, 139);
    var alt_foreground  = new Color(129, 161, 193);
    var alt_foreground2 = new Color(180, 142, 173);
    var alt_foreground3 = new Color(143, 188, 187);
    var alt_foreground4 = new Color(236, 239, 244);
    var fontSize        = 12;
    var fontName        = "mononoki NF";
    var barHeight       = 18;
    var gap             = 10;



    //* ****** *//
    //* CONFIG *//
    //* ****** *//
    context.CanMinimizeWindows = true;
    context.ToggleConsoleWindow(); //disable console on startup


    //* **** *//
    //* GAPS *//
    //* **** *//

    var gapPlugin = context.AddGap(new GapPluginConfig() { InnerGap = gap, OuterGap = gap / 2, Delta = gap / 2 });


    //* *** *//
    //* BAR *//
    //* *** *//

    context.AddBar(new BarPluginConfig()
    {
        //* Bar config
        FontSize = fontSize,
        BarHeight = barHeight,
        FontName = fontName,
        DefaultWidgetBackground = background,
        DefaultWidgetForeground = foreground,
        

        //* Left Widgets
        LeftWidgets = () => new IBarWidget[]
        {
            new TextWidget(" "), 
            new ActiveLayoutWidget()
            {
                UseAlias = true
            }, 
            new TextWidget("|"), 
            new WorkspaceWidget()
            {
                WorkspaceHasFocusColor = positive,
                WorkspaceEmptyColor = alt_background,
                WorkspaceIndicatingBackColor =  urgent,
            },
            new TextWidget("  "), 
            new TitleWidget() 
            {
                MonitorHasFocusColor = foreground3,
                IsShortTitle = true,
                NoWindowMessage = ""
            }
        },
        
        //* Right Widgets
        RightWidgets = () => new IBarWidget[]
        {
            new CpuPerformanceWidget() 
            {
                StringFormat = "" + "{0}%"  
            },
            new MemoryPerformanceWidget()
            {
                StringFormat = " " + "{0}%"  
            },
            new TextWidget(" "), 
            new BatteryWidget(),
            new TextWidget("  "),
            new TimeWidget(1000, "HH:mm dd/MM/yyyy"),
            new TextWidget(" "),
        }
    });


    //* ************ *//
    //* FOCUS BORDER *//
    //* ************ *//

    context.AddFocusBorder(new FocusBorderPluginConfig()
    {
        BorderColor = foreground3,
        BorderSize = 5,
        Opacity = 0.8

    });


    //* ******* *//
    //* LAYOUTS *//
    //* ******* *//

    //* Define Layout list
    Func<ILayoutEngine[]> defaultLayouts = () => new ILayoutEngine[]
    {
        new DwindleLayoutEngine(){
            Alias = "侀"
        },
        // new TallLayoutEngine(){
        //     Alias = "﬿"
        // },
        // new VertLayoutEngine(),
        // {
        //     Alias = "ﰧ"
        // }
        // new HorzLayoutEngine(){
        //     Alias = "ﰦ"
        // },
        new FullLayoutEngine(){
            Alias = ""
        },
        new FocusLayoutEngine(){
            Alias = "頻"
        },
    };

    //*Assign layout list to config context
    context.DefaultLayouts = defaultLayouts;


    //* ********** *//
    //* WORKSPACES *//
    //* ********** *//
    
    //* Define list of workspace names and layouts
    (string, ILayoutEngine[])[] workspaces =
    {
        ("爵 Web", defaultLayouts()),
        (" Chat", defaultLayouts()),
        (" Launcher",defaultLayouts()),
        (" Games", defaultLayouts()),
        (" Coding", defaultLayouts()),
        ("懶 Media", defaultLayouts()),
    };
    
    //* Creates one workspace for each of the layouts defined above
    foreach ((string name, ILayoutEngine[] layouts) in workspaces)
    {
        context.WorkspaceContainer.CreateWorkspace(name, layouts);
    }


    //* ******* *//
    //* FILTERS *//
    //* ******* *//

    //* By executable name
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("1Password.exe"));
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("pinentry.exe"));
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("AltSnap.exe"));
    context.WindowRouter.AddFilter((window) => !window.ProcessFileName.Equals("PowerToys.PowerLauncher.exe"));

    //* By window class
    context.WindowRouter.AddFilter((window) => !window.Class.Equals("ShellTrayWnd"));
    context.WindowRouter.AddFilter((window) => !window.Class.Equals("OperationStatusWindow")); //Explorer File copy
    context.WindowRouter.AddFilter((window) => !window.Class.Equals("#32770")); //File Selector

    //* By window title
    context.WindowRouter.AddFilter((window) => !window.Title.Equals("Color Picker"));


    //* *********** *//
    //* ACTION MENU *//
    //* *********** *//
    var actionMenu = context.AddActionMenu(new ActionMenuPluginConfig()
    {
        RegisterKeybind = false,
        MenuHeight = barHeight,
        FontSize = fontSize,
        FontName = fontName,
        Background = background,
    });

    //* Action menu builder
    Func<ActionMenuItemBuilder> createActionMenuBuilder = () =>
    {
        var menuBuilder = actionMenu.Create();

        //* Workspacer functions
        menuBuilder.Add("View keybinds", () => context.Keybinds.ShowKeybindDialog());
        menuBuilder.Add("Enable/Disable Workspacer", () => context.Enabled = !context.Enabled);
        menuBuilder.Add("Restart Workspacer", () => context.Restart());
        menuBuilder.Add("Quit Workspacer", () => context.Quit());

        return menuBuilder;
    };
    var actionMenuBuilder = createActionMenuBuilder();


    //* *********** *//
    //* KEYBINDINGS *//
    //* *********** *//

    Action setKeybindings = () =>
    {
        //* Keybinding setup
        // Declare modifiers and combos
        KeyModifiers winShift = KeyModifiers.Win | KeyModifiers.Shift;
        KeyModifiers winCtrl = KeyModifiers.Win | KeyModifiers.Control;
        KeyModifiers win = KeyModifiers.Win;
        // Initialize Keybind manager
        IKeybindManager manager = context.Keybinds;
        // declare variables
        var workspaces = context.Workspaces;

        // Disables all previous keybindings
        manager.UnsubscribeAll();

        //* Mouse bindings
        // Left click focuses monitor
        manager.Subscribe(MouseEvent.LButtonDown, () => workspaces.SwitchFocusedMonitorToMouseLocation());

        //* Workspace bindings
        // Automatic keybindings for workspaces 1-9
        foreach (IWorkspace workspace in context.WorkspaceContainer.GetWorkspaces(context.MonitorContainer.FocusedMonitor))
        {
            // Gets workspace index (first workspace is index 0)
            int i = context.WorkspaceContainer.GetWorkspaceIndex(workspace);
            // Winkey + numbers 1-9 switches to corresponding workspace (considering index+1)
            manager.Subscribe(win, (Keys)(49 + i), () => workspaces.SwitchToWorkspace(workspace), "Switch to Workspace " + (i+1));
            // Winkey +Shift + numbers 1-9 moves focused window to corresponding workspace (considering index+1)
            manager.Subscribe(winShift, (Keys)(49 + i), () => workspaces.MoveFocusedWindowToWorkspace(i), "Switch to Workspace " + (i+1));
        }
        // Winkey + Left/Right focuses previous/next workspace
        manager.Subscribe(win, Keys.Left, () => workspaces.SwitchToPreviousWorkspace(), "Switch to previous workspace");
        manager.Subscribe(win, Keys.Right, () => workspaces.SwitchToNextWorkspace(), "Switch to next workspace");
        // Winkey + Shift + Left/Right moves window to previous/next monitor
        manager.Subscribe(winShift, Keys.Left, () => workspaces.MoveFocusedWindowToPreviousMonitor(), "move focused window to previous monitor");
        manager.Subscribe(winShift, Keys.Right, () => workspaces.MoveFocusedWindowToNextMonitor(), "move focused window to next monitor");
        
        //* Layout management bindings
        // Winkey + Shift + H/L keys (vim keys), Shrinks/Expands primary area for current layout
        manager.Subscribe(winShift, Keys.H, () => workspaces.FocusedWorkspace.ShrinkPrimaryArea(), "Shrink primary area");
        manager.Subscribe(winShift, Keys.L, () => workspaces.FocusedWorkspace.ExpandPrimaryArea(), "Expand primary area");
        // Winkey + Ctrl + H/L keys (vim keys), Decreases/Increases primary window count for current layout
        manager.Subscribe(winCtrl, Keys.H, () => workspaces.FocusedWorkspace.DecrementNumberOfPrimaryWindows(), "Subtract primary windows");
        manager.Subscribe(winCtrl, Keys.L, () => workspaces.FocusedWorkspace.IncrementNumberOfPrimaryWindows(), "Add primary windows");
        // Winkey + Shift + K/J keys (vim keys), Moves window to next/previous position
        manager.Subscribe(winShift, Keys.K, () => workspaces.FocusedWorkspace.SwapFocusAndNextWindow(), "Move window to next position");
        manager.Subscribe(winShift, Keys.J, () => workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow(), "Move window to previous position");
        // Winkey + K/J keys (vim keys), Focuses next/previous window
        manager.Subscribe(win, Keys.K, () => workspaces.FocusedWorkspace.FocusNextWindow(), "Focus next window");
        manager.Subscribe(win, Keys.J, () => workspaces.FocusedWorkspace.FocusPreviousWindow(), "Focus previous window");
        // Winkey + Ctrl + Add(+)/Subtract(-) Resize inner gap
        manager.Subscribe(winCtrl, Keys.Add, () => gapPlugin.IncrementInnerGap(), "Increase inner gap");
        manager.Subscribe(winCtrl, Keys.Subtract, () => gapPlugin.DecrementInnerGap(), "Decrease inner gap");
        // Winkey + Shift + Add(+)/Subtract(-) Resize outer gap
        manager.Subscribe(winShift, Keys.Add, () => gapPlugin.IncrementOuterGap(), "Increase outer gap");
        manager.Subscribe(winShift, Keys.Subtract, () => gapPlugin.DecrementOuterGap(), "Decrease outer gap");

        //* Other shortcuts
        // Winkey + Ctrl + P Brings up action menu
        manager.Subscribe(winCtrl, Keys.P, () => actionMenu.ShowMenu(actionMenuBuilder), "Show workspacer menu");
        // Winkey + Ctrl + R Restarts workspacer
        manager.Subscribe(winCtrl, Keys.R, () => context.Restart(), "Restart workspacer");
        // Winkey + Shift + Escape Turns workspacer off/on
        manager.Subscribe(winShift, Keys.Escape, () => context.Enabled = !context.Enabled, "Toggle workspacer on/off");
        // Winkey + Shift + I Toggle workspacer log console
        manager.Subscribe(winShift, Keys.I, () => context.ToggleConsoleWindow(), "Toggle workspacer log console");
    };
    setKeybindings();
});
