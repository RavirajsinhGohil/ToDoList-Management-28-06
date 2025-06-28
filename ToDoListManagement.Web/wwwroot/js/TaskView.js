const today = new Date().toISOString().split('T')[0];
var startDateValidation = "";
var endDateValidation = "";
var startDateValidationEdit = "";
var endDateValidationEdit = "";
$(document).ready(function () {
    initSortable();

    $("#projectDropDown").change(function () {
        selectedProjectId = $(this).val();
        if (selectedProjectId) {
            GetTasksByProjectId(selectedProjectId);
        } else {
            $(".card-body").empty();
        }
    });

    $("#addTaskModal").on('hidden.bs.modal', function () {
        $("#addTaskForm").trigger("reset");
        $("#addTaskForm").find(".text-danger").text("");
    });
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("TaskStatusChanged", (taskId, newStatus) => {
    console.log(`Task ${taskId} changed status to ${newStatus}`);
    GetTasksByProjectId(selectedProjectId);
});

connection.on("NewTaskAdded", () => {
    console.log("New Task Added");
    GetTasksByProjectId(selectedProjectId);
});

connection.on("TaskUpdated", () => {
    console.log("Task Updated");
    GetTasksByProjectId(selectedProjectId);
});

connection.on("TaskDeleted", () => {
    console.log("Task Deleted");
    GetTasksByProjectId(selectedProjectId);
});

connection.start().catch(err => console.error(err.toString()));

function sendMessage() {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
}

function initSortable() {
    $(".task-column").sortable({
        connectWith: ".task-column",
        items: ".task-card",
        placeholder: "task-placeholder",
        receive: function (event, ui) {
            if (!window.TaskPermissions.canAddEdit) {
                $(ui.sender).sortable('cancel');
                toastr.warning("You do not have permission to update task status.");
                return;
            }
            const taskId = ui.item.data("task-id");
            const oldStatus = ui.sender.data("status");
            const newStatus = $(this).data("status");

            // Define allowed transitions
            const allowedTransitions = {
                "To Do": ["In Progress"],
                "In Progress": ["To Do", "Testing"],
                "Testing": ["In Progress", "Done"],
                "Done": ["Testing"]
            };

            // Validate transition
            if (!allowedTransitions[oldStatus] || !allowedTransitions[oldStatus].includes(newStatus)) {
                // Revert the move
                $(ui.sender).sortable('cancel');
                toastr.warning(`Tasks from "${oldStatus}" cannot be moved to "${newStatus}".`);
                return;
            }

            $.ajax({
                url: '/Tasks/UpdateStatus',
                type: 'POST',
                data: { taskId, newStatus },
                success: function (response) {
                    if (response.success) {
                        toastr.success(response.message);
                        connection.invoke("NotifyStatusChange", taskId, newStatus)
                            .catch(err => console.error("Error notifying clients:", err));
                    }
                    else {
                        toastr.error(response.message);
                    }
                },
                error: function () {
                    console.error("Error updating task status.");
                }
            });
        }
    }).disableSelection();
}

function GetTasksByProjectId(selectedProjectId) {
    $.ajax({
        url: '/Tasks/GetTasksByProjectId',
        type: 'GET',
        data: { projectId: selectedProjectId },
        success: function (data) {
            $("#taskListContainer").empty();
            $("#taskListContainer").append(data);
            initSortable();
        },
        error: function () {
            console.log("Error retrieving tasks.");
        }
    });
}

function openAddTaskModal() {
    if(!selectedProjectId)
    {
        toastr.error("Please select a Project");
        return;
    }
    $.ajax({
        url: '/Tasks/GetTeamMembers',
        type: 'GET',
        data: {
            projectId: selectedProjectId
        },
        success: function (response) {
            if (tinymce.get('addTaskDescription')) {
                tinymce.get('addTaskDescription').remove();
            }
            $("#addTaskModal").html(response);
            $("#addTaskModal").modal('show');
            $.validator.unobtrusive.parse("#addTaskForm");

            const startDateInput = $('#addTaskStartDate');

            startDateValidation = startDateInput.attr('min');
            endDateValidation = startDateInput.attr('max');
        },
        error: function () {
            console.error("Error while open task modal.");
        }
    });
}

function openAddTaskInputFile() {
    const fileUpload = $("#addTaskInputFile");
    fileUpload.click();
}

function openEditTaskInputFile() {
    const fileUpload = $("#editTaskInputFile");
    fileUpload.click();
}

function openEditTaskModal(taskId) {
    $.ajax({
        url: '/Tasks/GetTaskById',
        type: 'GET',
        data: { taskId: taskId },
        success: function (response) {
            if (tinymce.get('editTaskDescription')) {
                tinymce.get('editTaskDescription').remove();
            }
            $("#editTaskModal").html(response);
            $("#editTaskModal").modal('show');
            const startDateInputEdit = $('#editTaskStartDate');

            startDateValidationEdit = startDateInputEdit.attr('min');
            endDateValidationEdit = startDateInputEdit.attr('max');

            if (!window.TaskPermissions.canAddEdit) {
                $("#editTaskUpload").addClass('d-none');
                $("#editTaskForm")
                    .find("input, select, textarea, button[type='submit']")
                    .attr("disabled", true);
            }

            $.validator.unobtrusive.parse("#editTaskForm");

            tinymce.init({
                selector: '#editTaskDescription',
                height: 200,
                menubar: false,
                plugins: 'lists link image',
                toolbar: 'undo redo | formatselect | bold italic underline | alignleft aligncenter alignright | bullist numlist ',
                branding: false,
                statusbar: false,
                setup: function (editor) {
                    const maxLength = 1000;
        
                    editor.on('keydown', function (e) {
                        const content = editor.getContent({ format: 'text' });
        
                        const navigationalKeys = [
                            'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
                            'ArrowUp', 'ArrowDown', 'Control', 'Meta', 'Alt'
                        ];
        
                        if (content.length >= maxLength && !navigationalKeys.includes(e.key)) {
                            e.preventDefault();
                        }
                    });
        
                    editor.on('blur', function (e) {
                        const content = editor.getContent({ format: 'text' });
        
                        const navigationalKeys = [
                            'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
                            'ArrowUp', 'ArrowDown', 'Control', 'Meta', 'Alt'
                        ];
        
                        if (content.length >= maxLength && !navigationalKeys.includes(e.key)) {
                            editor.setContent(content.substring(0, 1000), {format : 'raw'});
                            e.preventDefault();
                            return false;
                        }
                    });
                }
            });
        },
        error: function () {
            console.error("Error retrieving task details.");
        }
    });
}

function openDeleteTaskModal(taskId) {
    $("#deleteTaskModal").modal('show');
    $("#deleteTaskLink").attr("href", `/Tasks/DeleteTask?taskId=${taskId}`);
}

const maxTotalSize = 25 * 1024 * 1024;
const allowedTypes = [
    'image/jpeg', 'image/png', 'image/gif', 'image/webp',
    'application/pdf',
    'application/msword',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'text/plain'
];

$(document).on('change', '#addTaskInputFile', function () {
    const files = this.files;
    let totalSize = 0;
    const fileNamesList = $('#addTaskFileNamesList');

    for (let i = 0; i < files.length; i++) {
        totalSize += files[i].size;

        if (!allowedTypes.includes(files[i].type)) {
            toastr.error(` ${files[i].name} file is not allowed.`);
            this.value = "";
            return;
        }
        fileNamesList.append(`<div class="me-2"><i class="fa-solid fa-file"></i>${files[i].name}</div>`);
    }

    if (totalSize > maxTotalSize) {
        toastr.error('Total file size exceeds 25 MB.');
        this.value = "";
    }
});

$(document).on('change', '#editTaskInputFile', function () {
    const files = this.files;
    let totalSize = 0;
    const fileNamesList = $('#editTaskFileNamesList');

    for (let i = 0; i < files.length; i++) {
        totalSize += files[i].size;

        if (!allowedTypes.includes(files[i].type)) {
            toastr.error(`${files[i].name} file is not allowed.`);
            this.value = "";
            return;
        }
        fileNamesList.append(`<div class="me-2"><i class="fa-solid fa-file"></i>${files[i].name}</div>`);
    }

    if (totalSize > maxTotalSize) {
        toastr.error('Total file size exceeds 25 MB.');
        this.value = "";
    }
});

$(document).on('change', '#addTaskStartDate', function () {
    const fromDate = $(this).val();
    if (fromDate) {
        $("#addTaskDueDate").attr("min", fromDate);
    } else {
        $("#addTaskDueDate").attr("min", startDateValidation);
    }
});

$(document).on('change', '#addTaskDueDate', function () {
    const toDate = $(this).val();
    if (toDate) {
        $("#addTaskStartDate").attr("max", toDate);
    } else {
        $("#addTaskStartDate").attr("max", endDateValidation);
    }
});

$(document).on('change', '#editTaskStartDate', function () {
    var fromDate = $(this).val();
    if (fromDate) {
        $("#editTaskDueDate").attr("min", fromDate);
    } else {
        $("#editTaskDueDate").attr("min", startDateValidationEdit);
    }
});

$(document).on('change', '#editTaskDueDate', function () {
    var toDate = $(this).val();
    if (toDate) {
        $("#editTaskStartDate").attr("max", toDate);
    } else {
        $("#editTaskStartDate").attr("max", endDateValidationEdit);
    }
});