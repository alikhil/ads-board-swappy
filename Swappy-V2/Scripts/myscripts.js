/*
    Скрипт для работы модальных окон и тултипа 
*/

$(function () {
    $title = $("#Title").val();
    $description = $("Description").val();
    $link = $("#ImageUrl").val();
    $checked = $("#AnotherVariants").val();

    $("#main-title-text").val($title);
    $("#main-description-text").val($description);
    $("#main-link-text").val($link);
    $("#main-another-variant-text").prop('checked', $checked);

    $('#itemEditModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); 
        var mode = button.attr("mode");
        var modal = $(this);
        modal.attr("mode", mode);
        var btnId = button.attr("btn-id");

        modal.attr("btn-id", btnId);

        if (mode == "edit")
        {
            var trueBtn = $("#" + btnId);
            var description = trueBtn.attr("data-tooltip");
            var title = trueBtn.text();
            modal.find('#title-text').val(title);
            modal.find('#description-text').val(description);
        }
        else {
            modal.find('#title-text').val("");
            modal.find('#description-text').val("");
        }
       
    });
   
    $('.main-save-btn').click(function (event)
    {
        $modal = $("#mainInfoModal");
        $title = $('#main-title-text').val();
        $description = $('#main-description-text').val();
        $link = $('#main-link-text').val();
        $checked = $('#main-another-variant-text').is(":checked");
        $.ajax({
            url: 'CheckItemValid',
            data: { "Title": $title, "Description": $description, "ImageUrl" : $link},
            success: function (result) {
                if (result == "ok") {
                    $("#main-modal-errors").html("").hide();
                    $("#Title").val($title);
                    $("#Description").val($description);
                    $("#ImageUrl").val($link);
                    $("#AnotherVariants").val($checked);

                    $("#ItemToChangeTitle").text($title);
                    $("#ItemToChangeDescription").text($description);
                    $("#ItemToChangeImage").attr("src", $link);
                    if($checked) 
                        $("#AnotherVariant_").text("Рассмотрю ваши варианты или обменяю на:").addClass("text-success");
                    else
                        $("#AnotherVariant_").text("Обменяю на:").removeClass("text-success");

                    $modal.modal("toggle");
                    
                }
                else {
                    $("#main-modal-errors").html(result).show();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert('oops, something bad happened');
            }
        });
        
    });
    $('.save-btn').click(function (event) {

            $modal = $("#itemEditModal");
            $mode = $modal.attr("mode");
            $title = $("#title-text").val();
            $description = $("#description-text").val();
           
            $oldBtnId = $modal.attr("btn-id");
            $item = $("#" + $oldBtnId);

            $id = $oldBtnId.split('-')[1];
             
            $.ajax({
                url: 'CheckItemValid',
                data: { "Id": $id, "Title": $title, "Description": $description },
                success: function (result) {
                    if (result == "ok") {
                        $("#modal-errors").html("").hide();
                        restoreInps($id);
                        $("#Variants_" + $id + "__Title").val($title).parent().show();
                        $("#Variants_" + $id + "__Description").val($description);
                        $("#" + $oldBtnId).text($title).attr("data-tooltip", $description);
                        $modal.modal("toggle");
                        $('.edit-button[btn-id*="itemBtn-' + $id + '"]').attr('mode', 'edit');
                    }
                    else {
                        $("#modal-errors").html(result).show();
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert('oops, something bad happened');
                }
            });

            
            
        }
    );
   initBtns();
});
function deleteInps(id)
{
    $id = id;
    $name = $("#Variants_" + $id + "__Id").attr("name");
    $("#Variants_" + $id + "__Id").attr("name", "Deleteted-" + $name);

    $namet = $("#Variants_" + $id + "__Title").attr("name");
    $("#Variants_" + $id + "__Title").attr("name", "Deleteted-" + $namet);

    $named = $("#Variants_" + $id + "__Description").attr("name");
    $("#Variants_" + $id + "__Description").attr("name", "Deleteted-" + $named);
}
function restoreInps(id)
{
    $id = id;
    $name = $("#Variants_" + $id + "__Id").attr("name").split('-')[1];
    $("#Variants_" + $id + "__Id").attr("name", $name);

    $namet = $("#Variants_" + $id + "__Title").attr("name").split('-')[1];
    $("#Variants_" + $id + "__Title").attr("name", $namet);

    $named = $("#Variants_" + $id + "__Description").attr("name").split('-')[1]
    $("#Variants_" + $id + "__Description").attr("name", $named);
}
function initBtns()
{
    $("[data-tooltip]").click(function (eventObject) {

        $data_tooltip = $(this).attr("data-tooltip");
        $pr = "<strong>Примечание:</strong><br/>"
        $("#tooltip").html($pr + $data_tooltip)
                     .css({
                         "top": $(this).offset().top + 25,
                         "left": $(this).offset().left
                     })
                     .toggle(300);

    }).focusout(function () {

        $("#tooltip").hide()
                     .text("")
                     .css({
                         "top": 0,
                         "left": 0
                     });
    });
    $('.restore-button').click(function (event) {
        $button = $(this); 
        $ids = $button.attr("btn-id");
        $id = $ids.split('-')[1];
        $(this).parent().children().each(function () {
            $anyItem = $(this);
            $anyItem.show();
        });

        restoreInps($id);
        $(this).hide();
    });
    $('.close-button').click(function (event) {
        var button = $(this); 
        $ids = button.attr('btn-id');
        $id = $ids.split('-')[1];
        $(button).parent().children().each(function () {
            $anyItem = $(this);
            $anyItem.hide();
        });

        deleteInps($id);

        $("#restoreBtn-" + $id).show();
    }
    );
}