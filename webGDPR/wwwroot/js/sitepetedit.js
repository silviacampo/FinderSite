function initCKEditor() {
  try {
    CKEDITOR.replace("FullDescription");
  }
  catch (err) {
    console.log(err);
  }
}

initCKEditor();