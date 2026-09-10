using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pawsome.API.Services.AI;

// Các lớp POCO map đúng cấu trúc JSON thật của Gemini REST API (models.generateContent,
// v1beta) - đã kiểm tra lại với tài liệu chính thức + notebook mẫu của Google trước khi viết
// (không đoán bừa cấu trúc request/response). Tên field giữ đúng snake_case/camelCase như
// Gemini yêu cầu, không đổi theo convention C#.

public class GeminiRequest
{
    [JsonPropertyName("system_instruction")]
    public GeminiContent? SystemInstruction { get; set; }

    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; set; } = new();

    [JsonPropertyName("tools")]
    public List<GeminiTool>? Tools { get; set; }

    [JsonPropertyName("generationConfig")]
    public GeminiGenerationConfig? GenerationConfig { get; set; }
}

public class GeminiGenerationConfig
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.3;

    [JsonPropertyName("maxOutputTokens")]
    public int MaxOutputTokens { get; set; } = 800;
}

public class GeminiContent
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = new();
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("functionCall")]
    public GeminiFunctionCall? FunctionCall { get; set; }

    [JsonPropertyName("functionResponse")]
    public GeminiFunctionResponse? FunctionResponse { get; set; }

    // Gemini 3.x bắt buộc: functionCall part đầu tiên của mỗi lượt (candidate.Content) phải mang lại
    // đúng thoughtSignature Gemini đã trả về khi lượt đó được gửi lại ở request kế tiếp (ChatAiService
    // gửi lại nguyên candidate.Content qua contents.Add) - thiếu field này ở model C# khiến giá trị bị
    // bỏ qua âm thầm lúc deserialize, rồi lượt gửi lại thiếu hẳn field, Gemini trả lỗi 400
    // "missing a thought_signature". Sibling với functionCall (không lồng bên trong) - đúng vị trí
    // theo tài liệu chính thức: https://ai.google.dev/gemini-api/docs/generate-content/thought-signatures
    [JsonPropertyName("thoughtSignature")]
    public string? ThoughtSignature { get; set; }
}

public class GeminiFunctionCall
{
    // Gemini 3.x gắn id cho mỗi lệnh gọi hàm khi có thể có NHIỀU lệnh gọi trong cùng 1 lượt - phải
    // echo lại đúng id này trong GeminiFunctionResponse tương ứng để model ghép đúng cặp gọi/kết quả
    // (model cũ hơn không gửi id, khi đó field này null và không cần echo lại).
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("args")]
    public JsonElement Args { get; set; }
}

public class GeminiFunctionResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("response")]
    public object Response { get; set; } = new();
}

public class GeminiTool
{
    [JsonPropertyName("function_declarations")]
    public List<GeminiFunctionDeclaration> FunctionDeclarations { get; set; } = new();
}

public class GeminiFunctionDeclaration
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("parameters")]
    public object Parameters { get; set; } = new();
}

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }

    [JsonPropertyName("promptFeedback")]
    public GeminiPromptFeedback? PromptFeedback { get; set; }
}

public class GeminiPromptFeedback
{
    [JsonPropertyName("blockReason")]
    public string? BlockReason { get; set; }
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; set; }
}
