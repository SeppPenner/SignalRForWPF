// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Error.cshtml.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The error class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SignalRForWPF.Server.Pages;

/// <summary>
/// The error class.
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    /// <summary>
    /// Gets or sets the request identifier.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// A value indicating whether the request identifier is shown or not.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

    /// <summary>
    /// Handles the get method.
    /// </summary>
    public void OnGet()
    {
        this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
    }
}
