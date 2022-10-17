import React from "react";
import { postInfo } from "src/models/postInfo";
import classes from "./PostCard.module.css"


const PostCard:React.FC<postInfo> = (props) =>{
    return(
        <span className={classes.postCard}>
            <img src={props.imageLink} className={classes.postCardImage} />
            <span className={classes.postCardTextPart}>
                <span className={classes.postCardTitle}>
                    {props.postTitle ?? "Untitled"}
                </span>
                <br/>
                <span className={classes.postCardSubtitle}>
                    {props.postSubtitle?? ""}
                </span>
            </span>
        </span>
    ) 
}

export default PostCard;