# ISSI_Reports

The ViewReport.ascx and ViewReport.ascx.vb files are modified to work with 3 parameters
User Selection
Start Date
End Date

Defaults to all users, with start date 7 days prior to current date and current date.

Page Load event sets the report query module setting so that the parameters are used.  This won't work with URL parameters because 
the settings for it via the module are not perfect.
