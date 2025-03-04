﻿using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Domain.DTOs.LogBook;
using Domain.Models;
using HttpClients.ClientInterfaces;

namespace HttpClients.Implementations;

public class LogBookHttpClient : ILogBookService
{
    private readonly HttpClient client;

    public LogBookHttpClient(HttpClient client)
    {
        this.client = client;
    }
    
    public async Task<LogBookEntity> CreateAsync(LogBookCreateDTO dto)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/logbook", dto);
        string result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(result);
        }

        LogBookEntity logBook = JsonSerializer.Deserialize<LogBookEntity>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        return logBook;
    }

    public async Task UpdateAsync(LogBookUpdateDTO dto)
    {
        string dtoAsJson = JsonSerializer.Serialize(dto);
        StringContent body = new StringContent(dtoAsJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PatchAsync("/api/logbook", body);
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }

    public async Task<LogBookEntity> GetByIdAsync(int id)
    {
        HttpResponseMessage response = await client.GetAsync($"/api/logbook/{id}");
        string result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(result);
        }

        LogBookEntity logBook = JsonSerializer.Deserialize<LogBookEntity>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        return logBook;
    }

    public async Task<LogBookEntity> GetByProjectIdAsync(int id)
    {
        HttpResponseMessage response = await client.GetAsync($"/api/logbook?id="+id);
        string result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(result);
        }

        LogBookEntity logBook = JsonSerializer.Deserialize<LogBookEntity>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        return logBook;
    }
}