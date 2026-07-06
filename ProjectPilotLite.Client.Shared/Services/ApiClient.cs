using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.Services;

public class ApiClient : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ProjectSummaryDto>> GetProjectsAsync(CancellationToken cancellationToken = default)
        => await GetAsync<List<ProjectSummaryDto>>("api/projects", cancellationToken) ?? [];

    public async Task<ProjectDetailDto?> GetProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await SendGetAsync($"api/projects/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProjectDetailDto>(JsonOptions, cancellationToken);
    }

    public async Task<ProjectDetailDto> CreateProjectAsync(ProjectCreateDto dto, CancellationToken cancellationToken = default)
        => await PostAsync<ProjectCreateDto, ProjectDetailDto>("api/projects", dto, cancellationToken);

    public Task UpdateProjectStatusAsync(Guid id, ProjectStatus status, CancellationToken cancellationToken = default)
        => PatchAsync($"api/projects/{id}/status", new ProjectStatusUpdateDto { Status = status }, cancellationToken);

    public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await GetAsync<List<TaskDto>>($"api/projects/{projectId}/tasks", cancellationToken) ?? [];

    public async Task<TaskDto> CreateTaskAsync(Guid projectId, TaskCreateDto dto, CancellationToken cancellationToken = default)
        => await PostAsync<TaskCreateDto, TaskDto>($"api/projects/{projectId}/tasks", dto, cancellationToken);

    public Task UpdateTaskStatusAsync(Guid taskId, ProjectTaskStatus status, CancellationToken cancellationToken = default)
        => PatchAsync($"api/tasks/{taskId}/status", new TaskStatusUpdateDto { Status = status }, cancellationToken);

    public async Task<IReadOnlyList<DeliverableDto>> GetDeliverablesAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await GetAsync<List<DeliverableDto>>($"api/projects/{projectId}/deliverables", cancellationToken) ?? [];

    public async Task<DeliverableDto> CreateDeliverableAsync(Guid projectId, DeliverableCreateDto dto, CancellationToken cancellationToken = default)
        => await PostAsync<DeliverableCreateDto, DeliverableDto>($"api/projects/{projectId}/deliverables", dto, cancellationToken);

    public Task UpdateDeliverableStatusAsync(Guid deliverableId, DeliverableStatus status, string? comment, CancellationToken cancellationToken = default)
        => PatchAsync($"api/deliverables/{deliverableId}/status", new DeliverableStatusUpdateDto { Status = status, Comment = comment }, cancellationToken);

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
        => await GetAsync<DashboardDto>("api/dashboard", cancellationToken) ?? new DashboardDto();

    private async Task<T?> GetAsync<T>(string uri, CancellationToken cancellationToken)
    {
        using var response = await SendGetAsync(uri, cancellationToken);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    private async Task<TResult> PostAsync<TBody, TResult>(string uri, TBody body, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(() => _http.PostAsJsonAsync(uri, body, JsonOptions, cancellationToken));
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<TResult>(JsonOptions, cancellationToken))!;
    }

    private async Task PatchAsync<TBody>(string uri, TBody body, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(() => _http.PatchAsJsonAsync(uri, body, JsonOptions, cancellationToken));
        await EnsureSuccessAsync(response);
    }

    private Task<HttpResponseMessage> SendGetAsync(string uri, CancellationToken cancellationToken)
        => SendAsync(() => _http.GetAsync(uri, cancellationToken));

    private static async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            return await send();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(
                "Impossible de contacter l'API. Vérifiez qu'elle est démarrée et que l'URL est correcte.",
                inner: ex);
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw new ApiException(await BuildErrorMessageAsync(response), response.StatusCode);
    }

    private static async Task<string> BuildErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(JsonOptions);
            if (problem?.Errors is { Count: > 0 })
            {
                return string.Join(Environment.NewLine, problem.Errors.SelectMany(entry => entry.Value));
            }

            if (!string.IsNullOrWhiteSpace(problem?.Title))
            {
                return problem.Title;
            }
        }
        catch (JsonException)
        {
            // Réponse d'erreur non structurée : on retombe sur le code HTTP.
        }

        return $"Erreur {(int)response.StatusCode} ({response.ReasonPhrase}).";
    }

    private sealed record ValidationProblemResponse
    {
        public string? Title { get; init; }
        public Dictionary<string, string[]>? Errors { get; init; }
    }
}
