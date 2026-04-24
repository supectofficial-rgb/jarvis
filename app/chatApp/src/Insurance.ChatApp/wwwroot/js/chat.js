(() => {
  const storeKeys = {
    sessionId: "chatapp.sessionId",
    userId: "chatapp.userId"
  };

  const state = {
    sessionId: null,
    userId: null,
    isSending: false,
    mediaRecorder: null,
    mediaStream: null,
    audioChunks: []
  };

  const dom = {
    messageList: document.getElementById("messageList"),
    composerInput: document.getElementById("composerInput"),
    sendBtn: document.getElementById("sendBtn"),
    recordBtn: document.getElementById("recordBtn"),
    stopRecordBtn: document.getElementById("stopRecordBtn"),
    newChatBtn: document.getElementById("newChatBtn"),
    reloadHistoryBtn: document.getElementById("reloadHistoryBtn"),
    sessionIdLabel: document.getElementById("sessionIdLabel"),
    userIdLabel: document.getElementById("userIdLabel"),
    connectionState: document.getElementById("connectionState"),
    micState: document.getElementById("micState")
  };

  document.addEventListener("DOMContentLoaded", init);

  async function init() {
    bindEvents();

    state.userId = getOrCreateUserId();
    state.sessionId = localStorage.getItem(storeKeys.sessionId);

    dom.userIdLabel.textContent = state.userId;
    setConnectionState("Connecting to Hub service...");

    await startSession(state.sessionId);
  }

  function bindEvents() {
    dom.sendBtn.addEventListener("click", onSend);
    dom.newChatBtn.addEventListener("click", () => startSession(null));
    dom.reloadHistoryBtn.addEventListener("click", loadHistory);
    dom.recordBtn.addEventListener("click", startRecording);
    dom.stopRecordBtn.addEventListener("click", stopRecording);

    dom.composerInput.addEventListener("keydown", event => {
      if (event.key === "Enter" && !event.shiftKey) {
        event.preventDefault();
        onSend();
      }
    });
  }

  async function startSession(existingSessionId) {
    const payload = {
      sessionId: existingSessionId,
      userId: state.userId
    };

    const { ok, result } = await apiCall("/chat/api/start", {
      method: "POST",
      body: JSON.stringify(payload)
    });

    if (!ok || !result?.isSuccess || !result?.data?.sessionId) {
      setConnectionState(result?.errorMessage || "Could not start session.");
      appendMessage("system", "Unable to start session. Please try again.");
      return;
    }

    state.sessionId = result.data.sessionId;
    localStorage.setItem(storeKeys.sessionId, state.sessionId);

    dom.sessionIdLabel.textContent = state.sessionId;
    setConnectionState("Connected");

    if (!existingSessionId) {
      clearMessages();
      appendMessage("assistant", "New session started. How can I help you today?");
    }

    await loadHistory();
  }

  async function loadHistory() {
    if (!state.sessionId) {
      return;
    }

    const query = new URLSearchParams({ sessionId: state.sessionId, take: "80" });
    const { ok, result } = await apiCall(`/chat/api/history?${query.toString()}`, { method: "GET" });

    if (!ok || !result?.isSuccess || !Array.isArray(result.data)) {
      return;
    }

    if (result.data.length === 0) {
      return;
    }

    clearMessages();
    result.data.forEach(msg => {
      const role = normalizeRole(msg.role);
      appendMessage(role, msg.content, { timestamp: msg.timestampUtc });
    });
  }

  async function onSend() {
    if (state.isSending) {
      return;
    }

    const text = dom.composerInput.value.trim();
    if (!text) {
      return;
    }

    if (!state.sessionId) {
      appendMessage("system", "Session is not ready yet. Try again.");
      return;
    }

    state.isSending = true;
    dom.sendBtn.disabled = true;

    appendMessage("user", text);
    dom.composerInput.value = "";

    const typingId = appendTyping();

    const payload = {
      sessionId: state.sessionId,
      userId: state.userId,
      text
    };

    const { ok, result } = await apiCall("/chat/api/message", {
      method: "POST",
      body: JSON.stringify(payload)
    });

    removeTyping(typingId);

    if (!ok || !result?.isSuccess || !result?.data) {
      appendMessage("system", result?.errorMessage || "Message failed.");
      state.isSending = false;
      dom.sendBtn.disabled = false;
      return;
    }

    const reply = result.data;
    const assistantMeta = buildAssistantMeta(reply.assistant);
    appendMessage("assistant", reply.assistantMessage || "Response received.", { meta: assistantMeta });

    state.isSending = false;
    dom.sendBtn.disabled = false;
    scrollToBottom();
  }

  async function startRecording() {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      appendMessage("system", "Browser audio recording is not supported.");
      return;
    }

    if (state.mediaRecorder && state.mediaRecorder.state === "recording") {
      return;
    }

    try {
      state.audioChunks = [];
      state.mediaStream = await navigator.mediaDevices.getUserMedia({ audio: true });
      state.mediaRecorder = new MediaRecorder(state.mediaStream);

      state.mediaRecorder.ondataavailable = event => {
        if (event.data && event.data.size > 0) {
          state.audioChunks.push(event.data);
        }
      };

      state.mediaRecorder.onstop = onRecordingStop;
      state.mediaRecorder.start();

      dom.recordBtn.disabled = true;
      dom.stopRecordBtn.disabled = false;
      dom.micState.textContent = "Recording...";
    } catch {
      appendMessage("system", "Microphone permission denied or unavailable.");
    }
  }

  function stopRecording() {
    if (!state.mediaRecorder || state.mediaRecorder.state !== "recording") {
      return;
    }

    state.mediaRecorder.stop();
    dom.recordBtn.disabled = false;
    dom.stopRecordBtn.disabled = true;
    dom.micState.textContent = "Processing voice...";
  }

  async function onRecordingStop() {
    try {
      const blob = new Blob(state.audioChunks, { type: state.mediaRecorder.mimeType || "audio/webm" });
      const base64Audio = await blobToBase64(blob);
      const extension = detectExtension(blob.type);

      if (!state.sessionId) {
        appendMessage("system", "Session is not ready yet. Voice transcription skipped.");
        return;
      }

      const payload = {
        sessionId: state.sessionId,
        userId: state.userId,
        audioBase64: base64Audio,
        extension,
        messageId: crypto.randomUUID()
      };

      const { ok, result } = await apiCall("/chat/api/transcribe", {
        method: "POST",
        body: JSON.stringify(payload)
      });

      if (!ok || !result?.isSuccess || !result?.data?.text) {
        appendMessage("system", result?.errorMessage || "Voice transcription failed.");
      } else {
        const text = result.data.text.trim();
        if (text) {
          dom.composerInput.value = dom.composerInput.value
            ? `${dom.composerInput.value} ${text}`
            : text;
          appendMessage("system", `Transcription ready: ${text}`);
        }
      }
    } finally {
      dom.micState.textContent = "Mic idle";
      if (state.mediaStream) {
        state.mediaStream.getTracks().forEach(track => track.stop());
        state.mediaStream = null;
      }
    }
  }

  function appendMessage(role, text, options = {}) {
    const item = document.createElement("article");
    item.className = `message ${role}`;

    const content = document.createElement("div");
    content.textContent = text;
    item.appendChild(content);

    if (options.meta && options.meta.length) {
      const meta = document.createElement("div");
      meta.className = "message-meta";
      options.meta.forEach(tag => {
        const badge = document.createElement("span");
        badge.className = `badge ${tag.kind || ""}`.trim();
        badge.textContent = tag.text;
        meta.appendChild(badge);
      });
      item.appendChild(meta);
    }

    dom.messageList.appendChild(item);
    scrollToBottom();
    return item.dataset.id;
  }

  function appendTyping() {
    const container = document.createElement("article");
    container.className = "message assistant";
    container.dataset.typingId = crypto.randomUUID();

    const typing = document.createElement("div");
    typing.className = "typing";
    typing.innerHTML = "<span></span><span></span><span></span>";

    container.appendChild(typing);
    dom.messageList.appendChild(container);
    scrollToBottom();
    return container.dataset.typingId;
  }

  function removeTyping(id) {
    const node = dom.messageList.querySelector(`[data-typing-id=\"${id}\"]`);
    if (node) {
      node.remove();
    }
  }

  function clearMessages() {
    dom.messageList.innerHTML = "";
  }

  function setConnectionState(text) {
    dom.connectionState.textContent = text;
  }

  function normalizeRole(role) {
    if (role === 1 || role === "User" || role === "user") return "user";
    if (role === 2 || role === "Assistant" || role === "assistant") return "assistant";
    return "system";
  }

  function buildAssistantMeta(assistant) {
    if (!assistant) {
      return [];
    }

    const tags = [];

    if (assistant.status) {
      tags.push({ text: assistant.status, kind: "status" });
    }

    if (Array.isArray(assistant.missingFields) && assistant.missingFields.length) {
      tags.push({ text: `Missing: ${assistant.missingFields.join(", ")}` });
    }

    if (Array.isArray(assistant.suggestions) && assistant.suggestions.length) {
      tags.push({ text: `Hints: ${assistant.suggestions.join(", ")}` });
    }

    return tags;
  }

  async function apiCall(url, options) {
    try {
      const response = await fetch(url, {
        headers: { "Content-Type": "application/json" },
        ...options
      });

      const result = await response.json().catch(() => null);
      return { ok: response.ok, result };
    } catch {
      return { ok: false, result: { isSuccess: false, errorMessage: "Network error." } };
    }
  }

  function getOrCreateUserId() {
    const existing = localStorage.getItem(storeKeys.userId);
    if (existing) {
      return existing;
    }

    const generated = `web-${crypto.randomUUID()}`;
    localStorage.setItem(storeKeys.userId, generated);
    return generated;
  }

  function detectExtension(mimeType) {
    if (!mimeType) {
      return "webm";
    }

    if (mimeType.includes("ogg")) {
      return "ogg";
    }

    if (mimeType.includes("mpeg") || mimeType.includes("mp3")) {
      return "mp3";
    }

    if (mimeType.includes("wav")) {
      return "wav";
    }

    return "webm";
  }

  function blobToBase64(blob) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onloadend = () => {
        const result = reader.result;
        if (typeof result !== "string") {
          reject(new Error("Unable to read audio payload."));
          return;
        }

        const commaIndex = result.indexOf(",");
        resolve(commaIndex >= 0 ? result.slice(commaIndex + 1) : result);
      };
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });
  }

  function scrollToBottom() {
    dom.messageList.scrollTop = dom.messageList.scrollHeight;
  }
})();
