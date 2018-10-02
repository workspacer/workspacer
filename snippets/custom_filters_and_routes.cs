# default implementation in the standard configuration
var router = new WindowRouter(context);
router.AddDefaults();
context.Workspaces.Router = router;

# custom filter, will ensure all windows with "my fun application" in the title are ignored
router.AddFilter((window) => !window.Title.Contains("my fun application"));

# custom route, will ensure that "Google Chrome" will be placed into the "web" workspace
router.AddRoute((window) => window.Title.Contains("Google Chrome") ? container["web"] : null));