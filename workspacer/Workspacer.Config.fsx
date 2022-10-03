//Dev
#r @"A:\Development\General\Github\workspacer\src\workspacer\bin\Release\net5.0-windows\win10-x64\workspacer.Shared.dll"
#r @"A:\Development\General\Github\workspacer\src\workspacer\bin\Release\net5.0-windows\win10-x64\plugins\workspacer.Bar\workspacer.Bar.dll"
#r @"A:\Development\General\Github\workspacer\src\workspacer\bin\Release\net5.0-windows\win10-x64\plugins\workspacer.Gap\workspacer.Gap.dll"
#r @"A:\Development\General\Github\workspacer\src\workspacer\bin\Release\net5.0-windows\win10-x64\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r @"A:\Development\General\Github\workspacer\src\workspacer\bin\Release\net5.0-windows\win10-x64\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"
#r @"A:\Development\General\Github\workspacer\src\workspacer\bin\Release\net5.0-windows\win10-x64\plugins\workspacer.FocusBorder\workspacer.FocusBorder.dll"

// Include plugins and use dependencies
// #r @"C:\Program Files\workspacer\workspacer.Shared.dll"
// #r @"C:\Program Files\workspacer\plugins\workspacer.Bar\workspacer.Bar.dll"
// #r @"C:\Program Files\workspacer\plugins\workspacer.Gap\workspacer.Gap.dll"
// #r @"C:\Program Files\workspacer\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
// #r @"C:\Program Files\workspacer\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"
// #r @"C:\Program Files\workspacer\plugins\workspacer.FocusBorder\workspacer.FocusBorder.dll"

open workspacer
open workspacer.Bar
open workspacer.Bar.Widgets
open workspacer.Gap
open workspacer.ActionMenu
open workspacer.FocusBorder

let setupContext (context : IConfigContext) =    
    //* ******* *//
    //* THEMING *//
    //* ******* *//

    let background      = Color(39,  46,   62)
    let urgent          = Color(191, 97,  106)
    let positive        = Color(163, 190, 140)
    let warning         = Color(235, 203, 139)
    let foreground      = Color(129, 161, 193)
    let foreground2     = Color(180, 142, 173)
    let foreground3     = Color(136, 192, 208)
    let foreground4     = Color(229, 233, 240)
    let alt_background  = Color(86,  106, 116)
    let alt_urgent      = Color(191, 97,  106)
    let alt_positive    = Color(163, 190, 140)
    let alt_warning     = Color(235, 203, 139)
    let alt_foreground  = Color(129, 161, 193)
    let alt_foreground2 = Color(180, 142, 173)
    let alt_foreground3 = Color(143, 188, 187)
    let alt_foreground4 = Color(236, 239, 244)
    let transparencykey = Color (12,12,12);
    let fontSize          = 12
    let fontName       = "mononoki NF"
    let barHeight         = 30
    let gapinner          = 10
    let gapouter          = 0
    let gapdelta          = 0


    //* ****** *//
    //* CONFIG *//
    //* ****** *//
    context.CanMinimizeWindows <- true
    context.ToggleConsoleWindow() //disable console on startup


    //* **** *//
    //* GAPS *//
    //* **** *//

    let gapPlugin = context.AddGap(fun c ->
        c.InnerGap <- gapinner
        c.OuterGap <- gapouter
        c.Delta <- gapdelta )

    //* *** *//
    //* BAR *//
    //* *** *//

    let barPlugin = context.AddBar(fun c ->
        c.FontSize                <- fontSize
        c.BarHeight               <- barHeight
        c.FontName                <- fontName
        c.DefaultWidgetBackground <- background
        c.DefaultWidgetForeground <- foreground
        c.IsTransparent           <- true
        c.TransparencyKey         <- transparencykey
        c.Background              <- background
        c.BarIsTop                <- false
        c.BarReservesSpace        <- true

        //* Left Widgets
        c.LeftWidgets <- fun () ->
            [| ActiveLayoutWidget()
               TextWidget("|")
               WorkspaceWidget
                 ( WorkspaceHasFocusColor = positive,
                   WorkspaceEmptyColor = alt_background,
                   WorkspaceIndicatingBackColor =  urgent )
               TextWidget("  ")
               TitleWidget
                 ( MonitorHasFocusColor = foreground3,
                   IsShortTitle = true,
                   NoWindowMessage = "" )
            |]
        
        //* Right Widgets
        c.RightWidgets <- fun () ->
            [| CpuPerformanceWidget(StringFormat = "" + "{0}%")
               MemoryPerformanceWidget(StringFormat = " " + "{0}%")
               TextWidget(" ")
               BatteryWidget()
               TextWidget("  ")
               TimeWidget(1000, "HH:mm dd/MM/yyyy")
            |]
    )


    //* ************ *//
    //* FOCUS BORDER *//
    //* ************ *//

    let focusBorderPlugin = context.AddFocusBorder(fun c ->
        c.BorderColor <- foreground3
        c.BorderSize <- 5
        c.Opacity <- 0.8
    )


    //* ******* *//
    //* LAYOUTS *//
    //* ******* *//

    //* Define Layout list
    let defaultLayouts : unit -> ILayoutEngine array = fun () ->
        [| DwindleLayoutEngine(1,0.65,0.03, Name = "侀")
           FullLayoutEngine (Name = "")
           FocusLayoutEngine(Name = "頻")
        // TallLayoutEngine (Name = "﬿")
        // VertLayoutEngine (Name = "ﰧ")
        // HorzLayoutEngine (Name = "ﰦ")
        |]

    //*Assign layout list to config context
    context.DefaultLayouts <- defaultLayouts


    //* ********** *//
    //* WORKSPACES *//
    //* ********** *//
    
    //* Define list of workspace names and layouts
    let workspaces =
        [| "爵 Web", defaultLayouts ()
           " Chat", defaultLayouts ()
           " Games", defaultLayouts()
           " Coding", defaultLayouts()
           " Etc",defaultLayouts()
           "懶 Media", defaultLayouts()
        |]
    
    for name, layouts in workspaces do
        context.WorkspaceContainer.CreateWorkspace(name, layouts)

    //* ******* *//
    //* FILTERS *//
    //* ******* *//

    //* By executable name
    context.WindowRouter.AddFilter(fun window -> window.ProcessFileName <> "1Password.exe")
    context.WindowRouter.AddFilter(fun window -> window.ProcessFileName <> "pinentry.exe")
    context.WindowRouter.AddFilter(fun window -> window.ProcessFileName <> "AltSnap.exe")
    context.WindowRouter.AddFilter(fun window -> window.ProcessFileName <> "PowerToys.PowerLauncher.exe")

    //* By window class
    context.WindowRouter.AddFilter(fun window -> window.Class <> "ShellTrayWnd")
    context.WindowRouter.AddFilter(fun window -> window.Class <> "OperationStatusWindow") //Explorer File copy
    context.WindowRouter.AddFilter(fun window -> window.Class <> "#32770") //File Selector

    //* By window title
    context.WindowRouter.AddFilter(fun window -> window.Title <> "Color Picker")


    //* *********** *//
    //* ACTION MENU *//
    //* *********** *//
    let actionMenu = context.AddActionMenu(fun c ->
        c.RegisterKeybind <- false
        c.MenuHeight <- barHeight
        c.FontSize <- fontSize
        c.FontName <- fontName
        c.Background <- background
    )

    //* Action menu builder
    let createActionMenuBuilder () =
        actionMenu.Create()
            .Add("View keybinds", fun () -> context.Keybinds.ShowKeybindDialog())
            .Add("Enable/Disable Workspacer", fun () -> context.Enabled <- not context.Enabled)
            .Add("Restart Workspacer", fun () -> context.Restart())
            .Add("Quit Workspacer", fun () -> context.Quit())
    let actionMenuBuilder = createActionMenuBuilder ()


    //* *********** *//
    //* KEYBINDINGS *//
    //* *********** *//

    let setKeybindings () =
        //* Keybinding setup
        // Declare modifiers and combos
        let winShift = KeyModifiers.Win ||| KeyModifiers.Shift
        let winCtrl = KeyModifiers.Win ||| KeyModifiers.Control
        let win = KeyModifiers.Win
        // Initialize Keybind manager
        let manager = context.Keybinds
        // declare variables
        let workspaces = context.Workspaces

        // Disables all previous keybindings
        manager.UnsubscribeAll()

        //* Mouse bindings
        // Left click focuses monitor
        manager.Subscribe(MouseEvent.LButtonDown, fun () -> workspaces.SwitchFocusedMonitorToMouseLocation())

        // Helper to convert int to key
        let key i : Keys = LanguagePrimitives.EnumOfValue i
        
        //* Workspace bindings
        // Automatic keybindings for workspaces 1-9
        for workspace in context.WorkspaceContainer.GetWorkspaces(context.MonitorContainer.FocusedMonitor) do
            // Gets workspace index (first workspace is index 0)
            let i = context.WorkspaceContainer.GetWorkspaceIndex(workspace)
            // Winkey + numbers 1-9 switches to corresponding workspace (considering index+1)
            manager.Subscribe(win, key (49 + i), (fun () -> workspaces.SwitchToWorkspace(workspace)), $"Switch to Workspace {i + 1}")
            // Winkey +Shift + numbers 1-9 moves focused window to corresponding workspace (considering index+1)
            manager.Subscribe(winShift, key (49 + i), (fun () -> workspaces.MoveFocusedWindowToWorkspace(i)), $"Switch to Workspace {i+1}")

        // Winkey + Left/Right focuses previous/next workspace
        manager.Subscribe(win, Keys.Left, (fun () -> workspaces.SwitchToPreviousWorkspace()), "Switch to previous workspace")
        manager.Subscribe(win, Keys.Right, (fun () -> workspaces.SwitchToNextWorkspace()), "Switch to next workspace")
        // Winkey + Shift + Left/Right moves window to previous/next monitor
        manager.Subscribe(winShift, Keys.Left, (fun () -> workspaces.MoveFocusedWindowToPreviousMonitor()), "move focused window to previous monitor")
        manager.Subscribe(winShift, Keys.Right, (fun () -> workspaces.MoveFocusedWindowToNextMonitor()), "move focused window to next monitor")
        
        //* Layout management bindings
        // Winkey + Shift + H/L keys (vim keys), Shrinks/Expands primary area for current layout
        manager.Subscribe(winShift, Keys.H, (fun () -> workspaces.FocusedWorkspace.ShrinkPrimaryArea()), "Shrink primary area")
        manager.Subscribe(winShift, Keys.L, (fun () -> workspaces.FocusedWorkspace.ExpandPrimaryArea()), "Expand primary area")
        // Winkey + Ctrl + H/L keys (vim keys), Decreases/Increases primary window count for current layout
        manager.Subscribe(winCtrl, Keys.H, (fun () -> workspaces.FocusedWorkspace.DecrementNumberOfPrimaryWindows()), "Subtract primary windows")
        manager.Subscribe(winCtrl, Keys.L, (fun () -> workspaces.FocusedWorkspace.IncrementNumberOfPrimaryWindows()), "Add primary windows")
        // Winkey + Shift + K/J keys (vim keys), Moves window to next/previous position
        manager.Subscribe(winShift, Keys.K, (fun () -> workspaces.FocusedWorkspace.SwapFocusAndNextWindow()), "Move window to next position")
        manager.Subscribe(winShift, Keys.J, (fun () -> workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow()), "Move window to previous position")
        // Winkey + K/J keys (vim keys), Focuses next/previous window
        manager.Subscribe(win, Keys.K, (fun () -> workspaces.FocusedWorkspace.FocusNextWindow()), "Focus next window")
        manager.Subscribe(win, Keys.J, (fun () -> workspaces.FocusedWorkspace.FocusPreviousWindow()), "Focus previous window")
        // Winkey + Ctrl + Add(+)/Subtract(-) Resize inner gap
        manager.Subscribe(winCtrl, Keys.Add, (fun () -> gapPlugin.IncrementInnerGap()), "Increase inner gap")
        manager.Subscribe(winCtrl, Keys.Subtract, (fun () -> gapPlugin.DecrementInnerGap()), "Decrease inner gap")
        // Winkey + Shift + Add(+)/Subtract(-) Resize outer gap
        manager.Subscribe(winShift, Keys.Add, (fun () -> gapPlugin.IncrementOuterGap()), "Increase outer gap")
        manager.Subscribe(winShift, Keys.Subtract, (fun () -> gapPlugin.DecrementOuterGap()), "Decrease outer gap")

        //* Other shortcuts
        // Winkey + Ctrl + P Brings up action menu
        manager.Subscribe(winCtrl, Keys.P, (fun () -> actionMenu.ShowMenu(actionMenuBuilder)), "Show workspacer menu")
        // Winkey + Ctrl + R Restarts workspacer
        manager.Subscribe(winCtrl, Keys.R, (fun () -> context.Restart()), "Restart workspacer")
        // Winkey + Shift + Escape Turns workspacer off/on
        manager.Subscribe(winShift, Keys.Escape, (fun () -> context.Enabled <- not context.Enabled), "Toggle workspacer on/off")
        // Winkey + Shift + I Toggle workspacer log console
        manager.Subscribe(winShift, Keys.I, (fun () -> context.ToggleConsoleWindow()), "Toggle workspacer log console")
    
    setKeybindings()
    
    ()
