import React, { forwardRef, useCallback, useEffect, useMemo, useState } from "react";
import apiHelper from "src/helpers/apiHelper";
import envHelper from "src/helpers/envHelper";
import { postInfo } from "src/models/postInfo";
import postsInfoRequest, { postType } from "src/models/postsInfoRequest";
import PostCard from "src/UI/components/postCard/PostCard";

const PostsList:React.FC<postsInfoRequest> = (props) => {
    const [currentPosts, setCurrentPosts] = useState<Array<postInfo>>([]);
    const onLoad = useEffect(() => {
        console.log("Calling onLoad in PostsList.")
        if (envHelper.isProduction()) {
            apiHelper.postsInfo.GetPostsInfos(props).then(result => {
                setCurrentPosts(result);
            });
        } else {
            const testPosts = new Array<postInfo>;
            testPosts.push({postTitle:`MyPost1 of ${props.postsType}`, postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost2", postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost3", postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost3", postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost3", postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost3", postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost3", postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost3", postSubtitle:"subtitle"});
            testPosts.push({postTitle:"MyPost3", postSubtitle:"subtitle"});
            setCurrentPosts(testPosts);
        }
    },[props.postsType]);
    return (
        <span style={{width:"100%",display:"flex",flexWrap:"wrap",justifyContent:"center"}}>
            {currentPosts.map((x, index) =>
                    <PostCard key={index} {...x} />
            )}
        </span>
    )
}

export default PostsList;