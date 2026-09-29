WHY THE DESIGNER WAS "LOCKED" AND WHAT WAS CHANGED
====================================================

THE PROBLEM
Every form except frmLogin and frmRegisterCustomer only had a constructor
that required a "LoginResult user" parameter, e.g.:

    public frmDashboard(LoginResult user) { ... }

The Visual Studio Windows Forms Designer needs a public PARAMETERLESS
constructor to create a live instance of the form for the design surface
(the drag-and-drop editor you see when you double-click a form).

Without one, Visual Studio cannot open these forms in Design view at all.
You're forced to edit the raw generated code by hand, and any small mistake
there breaks the build. That's the "everything is locked, if I change
anything the code doesn't work" problem.

THE FIX
For every affected form (frmDashboard, frmManageProducts, frmAdminProducts,
frmAdminReports, frmBrowseProducts, frmCustomerProfile, frmManageCustomers,
frmManageManagers, frmManageOrders, frmMyOrders, frmPayment), two things
were added to the .cs (NOT the .Designer.cs) file:

1. A parameterless constructor, used only by the Designer:

    public frmDashboard() : this(null) { }

2. A guard at the top of the real constructor so that database calls,
   null-checks, and role validation never run when the Designer is the one
   creating the form:

    if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        return;

Nothing about how the app behaves when you actually run it has changed.
The real constructors still require a LoginResult and still do everything
they did before; the guard only skips that logic when Visual Studio itself
is instantiating the form to draw the design surface.

WHAT YOU CAN DO NOW
- Double-click any form in Solution Explorer -> it opens in the visual
  Designer like normal.
- Click any control (button, label, panel, textbox...) and change its
  BackColor, ForeColor, Font, Text, size, or position in the Properties
  window, or drag it around -> Visual Studio updates the matching
  .Designer.cs file automatically. You no longer need to hand-edit that
  file.
- Build/run the app as usual afterward; nothing else was touched.

If a form still won't render after this, right-click it in Solution
Explorer -> "Open With..." isn't needed; just try Build -> Rebuild Solution
first, since the Designer needs the project to compile at least once to
load control types.

DASHBOARD SPLIT (Admin / Manager / Customer)
=============================================
The old single frmDashboard.cs (with three GroupBox panels toggled visible/
invisible by role) has been replaced with three fully separate forms:

  - Forms/frmAdminDashboard.cs    (Manage Managers, Admin Reports,
                                    Manage Customers, View Products)
  - Forms/frmManagerDashboard.cs  (Manage Products, Manage Orders)
  - Forms/frmCustomerDashboard.cs (Browse Products, My Orders, My Profile)

Each one is a normal, independent form with its own Designer.cs, so you can
open any of them in the Visual Studio Designer and change colors, fonts,
text, and layout without touching the other two.

frmLogin.cs now opens the matching dashboard based on the logged-in user's
role instead of always opening frmDashboard.

Note: the old frmDashboard.cs also built its buttons using a custom
"ConfigureButton(...)" helper method inside InitializeComponent. The Visual
Studio Designer cannot parse arbitrary method calls like that, so it
wouldn't have opened in Design view even after the earlier constructor fix.
The three new forms use plain, explicit property assignments per control
(the same pattern as frmLogin), so they load in the Designer normally.
