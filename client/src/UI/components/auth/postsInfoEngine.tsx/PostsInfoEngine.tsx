import React, { useEffect, useState } from "react"
import apiHelper from "src/helpers/apiHelper";
import envHelper from "src/helpers/envHelper";
import { postInfo } from "src/models/postInfo";
import postsInfoRequest from "src/models/postsInfoRequest"

export interface IPostsInfoEngine {
    req: postsInfoRequest;
    element: React.FC<Array<postInfo>>;
}

const PostsInfoEngine: React.FC<IPostsInfoEngine> = (props) => {
    const getPosts = () => {
        console.log("Calling onLoad in PostsList 2.");
        if (envHelper.isProduction()) {
            apiHelper.postsInfo.GetPostsInfos(props.req).then(result => {
                return (result);
            });
        } else {
            console.log("Adding empty posts for debugging");
            const testPosts = new Array<postInfo>;
            testPosts.push({ postTitle: `MyPost1 of ${props.req?.postsType ?? "any"}`, postSubtitle: "subtitle" });
            testPosts.push({ postTitle: "MyPost2", postSubtitle: "subtitle" });
            testPosts.push({ postTitle: "MyPost3", postSubtitle: "subtitle" });
            return (testPosts);
        }
        return [];
    }

    console.log("In PostsInfoEngine function.");
    const [currentPosts, setCurrentPosts] = useState<Array<postInfo>>(getPosts());

    const onLoad = useEffect(() => {setCurrentPosts(getPosts())}, []);

    console.log("returning from PostsInfoEngine");
    return React.createElement(props.element, currentPosts);
}

export default PostsInfoEngine;