import React, { useState } from "react"
import { ReactDOM } from "react";
import { Editor, EditorState } from "draft-js";
import 'draft-js/dist/Draft.css';
import ReactQuill from "react-quill";
import 'react-quill/dist/quill.snow.css';

const PostEditor: React.FC = () => {
    const [postEditorState, setEditorState]
        = useState("");

    return (
        <span style={{ width: "100%",margin:"10px",display:"flex", justifyContent: "center" }}>
            <ReactQuill
                value={postEditorState}
                onChange={newState => { console.log(newState); setEditorState(newState); }}
                theme="snow"
                style={{ width: "100%", height: "100%", maxWidth: "800px", display: "block" }}
            />
            </span>
    );
}

export default PostEditor;