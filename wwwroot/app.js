const listEl = document.getElementById("todo-list");
const formEl = document.getElementById("add-form");
const inputEl = document.getElementById("title-input");
const statsEl = document.getElementById("stats");
const emptyEl = document.getElementById("empty-state");
const template = document.getElementById("todo-template");

async function loadTodos() {
  const response = await fetch("/api/todos");
  const todos = await response.json();
  renderTodos(todos);
}

function renderTodos(todos) {
  listEl.replaceChildren();

  for (const todo of todos) {
    const node = template.content.cloneNode(true);
    const item = node.querySelector(".todo-item");
    const checkbox = node.querySelector('input[type="checkbox"]');
    const title = node.querySelector(".todo-title");
    const deleteBtn = node.querySelector(".delete-btn");

    title.textContent = todo.title;
    checkbox.checked = todo.isComplete;
    item.classList.toggle("complete", todo.isComplete);

    checkbox.addEventListener("change", () => toggleTodo(todo.id, checkbox.checked, item));
    deleteBtn.addEventListener("click", () => deleteTodo(todo.id));

    listEl.appendChild(node);
  }

  const open = todos.filter((t) => !t.isComplete).length;
  statsEl.textContent = `${todos.length} item${todos.length === 1 ? "" : "s"} · ${open} open`;
  emptyEl.classList.toggle("hidden", todos.length > 0);
}

async function toggleTodo(id, isComplete, itemEl) {
  itemEl.classList.toggle("complete", isComplete);

  await fetch(`/api/todos/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ isComplete }),
  });

  await loadTodos();
}

async function deleteTodo(id) {
  await fetch(`/api/todos/${id}`, { method: "DELETE" });
  await loadTodos();
}

formEl.addEventListener("submit", async (event) => {
  event.preventDefault();

  const title = inputEl.value.trim();
  if (!title) return;

  await fetch("/api/todos", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ title }),
  });

  inputEl.value = "";
  inputEl.focus();
  await loadTodos();
});

loadTodos().catch((error) => {
  emptyEl.textContent = "Could not load todos. Check DATABASE_URL and try again.";
  emptyEl.classList.remove("hidden");
  console.error(error);
});
