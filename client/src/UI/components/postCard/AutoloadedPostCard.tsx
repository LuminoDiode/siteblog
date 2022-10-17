import React from "react";
import apiHelper from "src/helpers/apiHelper";
import { postInfo } from "src/models/postInfo";
import postsInfoRequest from "src/models/postsInfoRequest";
import classes from "./PostCard.module.css"
import PostCard from "./PostCard";

export interface IPostLoadInfo {
    postId: number;
}

const AutoloadPostCard: React.FC<IPostLoadInfo> = (props) => {
    apiHelper.postsInfo.GetPostById(props.postId).then(Post => {
        return (
            <PostCard {...Post} />
        )
    });

    throw `Cannot fetch post with id = ${props.postId}`;
}

export default AutoloadPostCard;