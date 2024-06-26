﻿//------------------------------------------------------------- variables Section
const eachCHUNK_smallModule = 1000; // Based on KB // e.g. if you add 1000 in this variable, it is equalavent to (1000*1024)=>Byte // the volume of each chunk for uploading // NOTE: the last part may less than 100kb
var fileStorage = []; // retrive from fileupload array
//////////////////////////////////////////////////////
$('.kingMultiUplaoder').each(function () {
    const fileInput = $(this).find('input[type="file"]');
    fileInput.on('change', function () {
        // Clear the previous file entry from the fileStorage array
        var fileInputId = $(this).attr("id");
        fileStorage = fileStorage.filter(function (entry) {
            return entry.key !== fileInputId;
        });

        // Get the new file information and add it to the fileStorage array
        var file = $(this)[0].files[0];
        var fileEntry = {
            key: fileInputId,
            value: file
        };
        fileStorage.push(fileEntry);
        handleFileUpload(fileInputId);
    });
});
//-------------------------------------------------------------
// get the fileupload handler
function handleFileUpload(fileInputId) {
    //retrive from fileupload array
    var fileEntry = fileStorage.find(function (entry) {
        return entry.key === fileInputId;
    });
    var file = fileEntry.value; // Retrieve the file object from the file entry
    // check the file extension and size
    var checkSomeErrors = checkStandardVolumeExtentsion(fileInputId);
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

    CalcIncreaseValue(file.size, fileInputId); // [increased value] to progress of progressbar
    actionProgressbar(true, fileInputId); // the main Progressbar is started!
    var chunk = file.slice(0, 0 + chunkSize);
    uploadChunk(chunk, chunkSize, file.name, filePartCount, fileInputId, specificfoldername); // the main function is fired!
}

//upload
function uploadChunk(chunk, chunkSize, filename, filePartCount, fileInputId, specificfoldername) {
    var postData = new FormData();
    postData.append("file", chunk);
    postData.append("TotalFileSize", $("#" + fileInputId).prop('files')[0].size); // total volume of the file
    postData.append("OriginalFileExtension", $("#" + fileInputId).prop('files')[0].name.split('.').pop().toLowerCase()); // original extension
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
                alert('test');
                var chunkSize = $("#" + fileInputId).data("chunkSize");
                var start = $("#" + fileInputId).data("start");
                start += chunkSize;
                $("#" + fileInputId).data("start", start);

                var chunk2 = file.slice(start, start + chunkSize);

                if (chunk2.size >= chunkSize) {
                    actionProgressbar(true, fileInputId);
                }
                else {
                    chunkSize = chunk2.size;
                    chunk2 = file.slice(start, start + chunkSize);
                    actionProgressbar(false, fileInputId); // if the last part is less than [eachCHUNK_smallModule] KB
                }

                if (start < file.size) {
                    uploadChunk(chunk2, chunkSize, file.name, $("#" + fileInputId).data("filePartCount"), fileInputId, $("#" + fileInputId).data("specificFolderName"));
                }
            }
            if (data.result == 2) {
                merging(file.name, $("#" + fileInputId).data("specificFolderName"));
                actionProgressbar(false, fileInputId);
            }
        },
        error: function (request, status, error) {
        }
    });
}

//Progressbar actio
function actionProgressbar(lastPartState, fileInputId) {

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
function checkStandardVolumeExtentsion(fileInputId) {
    // if you want to check your on-server-validations, just use "return null;" and simply comment the follwing block!
    // get attribute values from the controls
    var maxVolumeAttribute = $("#" + fileInputId).attr("data-maxVolume");
    var extensionAttribute = $("#" + fileInputId).data("extensions");
    var extensionArray = extensionAttribute.replace(/[{}]/g, '').split(',');

    var checkSize = parseInt($("#" + fileInputId).prop('files')[0].size) < parseInt(maxVolumeAttribute);
    var checkExtension = extensionArray.includes($("#" + fileInputId)[0].files[0].name.split('.').pop().toLowerCase());

    // check the file extension
    if (checkExtension) {
        if (checkSize) return null; // Everything is OK
        else return "(client side) => Check your file size!"; // Error
    } else return "(client side) => Check your file extension!"; // Error
}

// CalcIncreaseValue_smallModule
function CalcIncreaseValue(fileSize, fileInputId) {
    var kb = fileSize / 1024; // convert to KB
    var eachKb = Math.ceil(kb / eachCHUNK_smallModule); // how many places does it have in 100% progressbar?
    var eachUnit = 100 / eachKb; // how much KB does include in each part (eachKb)?
    $("#" + fileInputId).data("increasedValue", eachUnit);
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
