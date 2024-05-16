$(document).ready(function () {

    $(document).on("click", ".load-more-comment", function () {
        console.log("loadmore")

        $.ajax({
            url: "/Book/LoadMore",
            type: "GET",
            success: function (res) {
                console.log(res)
            }
        });
    });

});