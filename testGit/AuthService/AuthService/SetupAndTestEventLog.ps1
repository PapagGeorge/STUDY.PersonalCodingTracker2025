# Run as Administrator!

$logName = "AuthService"
$sourceName = "AuthService"

# Check if the source exists
if ([System.Diagnostics.EventLog]::SourceExists($sourceName)) {
    Write-Host "Source '$sourceName' already exists. Deleting it first..."
    try {
        [System.Diagnostics.EventLog]::DeleteEventSource($sourceName)
        Write-Host "Source deleted successfully."
    }
    catch {
        Write-Warning "Could not delete source $sourceName. It might be in use or tied to a log with the same name."
    }
}

# Check if the log exists
if ([System.Diagnostics.EventLog]::Exists($logName)) {
    Write-Host "Event log '$logName' already exists. Deleting it first..."
    try {
        [System.Diagnostics.EventLog]::Delete($logName)
        Write-Host "Event log deleted successfully."
    }
    catch {
        Write-Warning "Could not delete event log $logName. It might be in use."
    }
}

# Create the event log and source
Write-Host "Creating event log '$logName' and source '$sourceName'..."
try {
    [System.Diagnostics.EventLog]::CreateEventSource($sourceName, $logName)
    Write-Host "Event log and source created successfully."
}
catch {
    Write-Warning "Could not create event log or source: $_"
    exit
}

# Write a test event to the new log
Write-Host "Writing a test event..."
try {
    Write-EventLog -LogName $logName -Source $sourceName -EntryType Information -EventId 1001 -Message "Test log entry from PowerShell script."
    Write-Host "Test event written successfully."
}
catch {
    Write-Warning "Failed to write test event: $_"
}

Write-Host "Script complete! Please restart your machine to ensure changes take effect."
