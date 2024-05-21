//------------------------------------------------------------- variables Section
const eachCHUNK_smallModule = 1000; // Based on KB // e.g. if you add 1000 in this variable, it is equalavent to (1000*1024)=>Byte // the volume of each chunk for uploading // NOTE: the last part may less than 100kb
var fileStorage = []; // retrive from fileupload array
//////////////////////////////////////////////////////
$('.kingMultiUplaoder').each(function () {
    const fileInput = $(this).find('input[type="file"]');
    fileInput.on('change', function () {
        var fileInputId = $(this).attr("id");
        var file = $(this)[0].files[0];
        var fileEntry = {
            key: fileInputId,
            value: file
        };
        fileStorage.push(fileEntry); // Store the file entry in the fileStorage array
        handleFileUpload_smallModule(fileInputId);
    });
});
//-------------------------------------------------------------
// get the fileupload handler
function handleFileUpload_smallModule(fileInputId) {
    //retrive from fileupload array
    var fileEntry = fileStorage.find(function (entry) {
        return entry.key === fileInputId;
    });
    var file = fileEntry.value; // Retrieve the file object from the file entry
    // check the file extension and size
    var checkSomeErrors = checkStandardVolumeExtentsion_smallModule(file);
    if (checkSomeErrors) {
        alert(checkSomeErrors);
        return;
    }
    var chunkSize = 1024 * eachCHUNK_smallModule; // size of each chunk (1MB)
    $("#" + fileInputId).data("chunkSize", 1024 * eachCHUNK_smallModule);
    var filePartCount = Math.ceil(file.size / chunkSize);
    $("#" + fileInputId).data("filePartCount", filePartCount);
    var specificfoldername = uuidv4();
    $("#" + fileInputId).data("specificFolderName", specificfoldername);

    CalcIncreaseValue_smallModule(file.size, fileInputId); // [increased value] to progress of progressbar
    actionProgressbar_smallModule(true, fileInputId); // the main Progressbar is started!
    var chunk = file.slice(0, 0 + chunkSize);
    uploadChunk_smallModule(chunk, chunkSize, file.name, filePartCount, fileInputId, specificfoldername); // the main function is fired!
}

//upload
function uploadChunk_smallModule(chunk, chunkSize, filename, filePartCount, fileInputId, specificfoldername) {
    var postData = new FormData();
    postData.append("file", chunk);
    postData.append("chunkSize", chunkSize);
    postData.append("Filename", filename);
    postData.append("start", $("#" + fileInputId).data("start"));
    postData.append("filePartCount", filePartCount);
    postData.append("SpecificFolderName", specificfoldername);

    $.ajax({
        contentType: false,
        processData: false,
        type: 'POST',
        data: postData,
        url: '/Home/UploadMultiFiles',
        success: function (data) {
            if (data.result == 0) {
                alert(data.message);
                location.reload();
            }

            /////////////////////

            var fileEntry = fileStorage.find(function (entry) {
                return entry.key === fileInputId;
            });
            var file = fileEntry.value; // Retrieve the file object from the file entry
            /////////////////////

            if (data.result == 1) {

                var chunkSize = $("#" + fileInputId).data("chunkSize");
                var start = $("#" + fileInputId).data("start");
                start += chunkSize;
                $("#" + fileInputId).data("start", start);

                var chunk2 = file.slice(start, start + chunkSize);

                if (chunk2.size >= chunkSize) {
                    actionProgressbar_smallModule(true, fileInputId);
                }
                else {
                    chunkSize = chunk2.size;
                    chunk2 = file.slice(start, start + chunkSize);
                    actionProgressbar_smallModule(false, fileInputId); // if the last part is less than [eachCHUNK_smallModule] KB
                }

                if (start < file.size) {
                    uploadChunk_smallModule(chunk2, chunkSize, file.name, $("#" + fileInputId).data("filePartCount"), fileInputId, $("#" + fileInputId).data("specificFolderName"));
                }
            }
            if (data.result == 2) {
                merging(file.name, $("#" + fileInputId).data("specificFolderName"));
                actionProgressbar_smallModule(false, fileInputId);
            }
        },
        error: function (request, status, error) {
        }
    });
}

//Progressbar actio
function actionProgressbar_smallModule(lastPartState, fileInputId) {

    var progressNumber = $('.' + fileInputId).siblings('.progressNumber').attr('id');
    var curprogress = $('.' + fileInputId).siblings('.curprogress').attr('id');

    if (lastPartState) {

        var total = parseInt($("#" + fileInputId).attr("data-currentProgress")) + parseInt($("#" + fileInputId).data("increasedValue"));

        $("#" + fileInputId).attr("data-currentProgress", total.toString());
        var currentProgress = total;

        var showedValue = parseFloat(currentProgress.toFixed(2));

        $("#" + curprogress).css("width", `${currentProgress}%`);
        $("#" + progressNumber).text(`${showedValue}%`);
    }
    else {
        $("#" + curprogress).css("width", "100%");
        $("#" + progressNumber).text("100%");
    }
}

// check the file size and extension
function checkStandardVolumeExtentsion_smallModule(fileInput) {

    var eVolume = "200000000";
    var eExtension = "jpg";

    var file_extension = /[^.]+$/.exec(fileInput.name); // get only the file extension

    // check the file size at first
    if (fileInput.size < eVolume) {
        // check the file extension
        if (eExtension.toLowerCase() == file_extension.toString().toLowerCase()) {
            return null;// everything is Ok
        }
        else {
            fileInput.value = null;
            return "(client side) => Check your file extension!"; // error
        }
    }
    else {
        fileInput.value = null;
        return "(client side) => Check your file size!"; // error
    }
}

// CalcIncreaseValue_smallModule
function CalcIncreaseValue_smallModule(fileSize, fileInputId) {
    var kb = fileSize / 1024; // convert to KB
    var eachKb = Math.ceil(kb / eachCHUNK_smallModule); // how many places does it have in 100% progressbar?
    var eachUnit = 100 / eachKb; // how much KB does include in each part (eachKb)?
    $("#" + fileInputId).data("increasedValue", eachUnit);
}

// delete database and files
function DeleteDatabaseAndMultiFiles() {
    $.ajax({
        content: 'application/x-www-form-urlencoded',
        contentType: 'json',
        type: 'POST',
        url: '/Home/DeleteMultiFiles',
        success: function (data) {
        },
        error: function (request, status, error) {
            //alert('request:' + request.responseText + ';err:' + error);
        }
    });
}

// merging_smallModule
function merging(filename,specificfoldername) {
    $.ajax({
        contentType: 'application/x-www-form-urlencoded',
        dataType: 'json',
        data: { filename: filename, specificfoldername: specificfoldername },
        type: 'POST',
        url: '/Home/MergeEachFile',
        async: true,
        success: async function () {
            //alert('1');
        },
        error: function (request, status, error) {
            //alert('request:' + request.responseText + ';err:' + error);
        }
    });
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-xxxx-yxxx-xxxxxxxxxxxx'
        .replace(/[xy]/g, function (c) {
            const r = Math.random() * 16 | 0,
                v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
}